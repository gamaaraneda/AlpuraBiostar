using AlpuraBiostar.Datos.Asistencia;
using AlpuraBiostar.Datos.Types;
using AlpuraBiostar.Datos.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Negocio.Asistencia
{
    public class NegocioAsistencia : INegocioAsistencia
    {
        IRepositorioAsistencia _repositorioAsistencia;
        private string _conexionWSAlpura;

        public NegocioAsistencia(string conexionMariaDB, string conexionWSAlpura)
        {

            _repositorioAsistencia = new RepositorioAsistencia(conexionMariaDB);
            _conexionWSAlpura = conexionWSAlpura;
        }

        public bool syncRegistros()
        {
            try
            {
                var listRegistros = _repositorioAsistencia.obtenerRegistrosNoSync();
                var result = syncRegistroOracle(listRegistros);
                return result;
            }
            catch (Exception EX)
            {

                return false;
            }
        }

        public List<TypeAsistencia> obtenerRegistrosPorRangoDeFechas(string fechaInicio, string fechaFin)
        {
            return _repositorioAsistencia.obtenerRegistrosPorRangoDeFechas(fechaInicio, fechaFin);
        }


        public bool syncRegistroOracle(List<TypeAsistencia> lstasistencias)
        {
            
            try
            {
                int contador = 1;
                int total = lstasistencias.Count;

                Log("Iniciando el envio de "+ total.ToString());
                foreach (var registro in lstasistencias)
                {
                    registro.Fecha = new Validaciones().validarFecha(registro.Fecha);
                    registro.Records = new Validaciones().validarFecha(registro.Records);

                    var registroAEnviar = crearRegistro(registro);

                    var json = JsonConvert.SerializeObject(registroAEnviar);
                    StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");

                    Log("registro  "+contador.ToString()+" / "+ total +" -> "+json);

                    using (var client = new HttpClient())
                    {
                        client.Timeout = new TimeSpan(0, 1, 0);
                        client.BaseAddress = new Uri(_conexionWSAlpura);

                        Log("registro  " + contador.ToString() + " / " + total +" -> Iniciando Conexion con ws");

                        var response = client.PostAsync("biostar_sirhal/", payload).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            Log("registro  " + contador.ToString() + " / " + total +" -> Se obtuvo respuesta de WS");
                            try
                            {

                           
                            var responseContent = response.Content;
                            string responseString = responseContent.ReadAsStringAsync().Result;

                            Log("registro  " + contador.ToString() + " / " + total  + " -> Se obtuvo respuesta de WS -> "+ responseString);

                            var resultadoOracle = JsonConvert.DeserializeObject<TypeResultOracle>(responseString);

                            Log("registro  " + contador.ToString() + " / " + total +  json + "-> Se desarializo correctamente");

                            registrarEstadoDeAsistencia(registro, resultadoOracle);
                            }
                            catch (Exception)
                            {

                                
                            }
                        }
                    }
                    contador++;
                }
                return true;
            }
            catch (Exception EX)
            {

                return false;
            }
        }

        private TypeRegistro crearRegistro(TypeAsistencia asistencia)
        {
            var registro = new TypeRegistro()
            {
                p_employee_number = asistencia.EmployeNumber,
                p_fecha = asistencia.Fecha,
                p_ubicacion = asistencia.Ubicacion,
                p_hostname = asistencia.Hostname,
                p_record = asistencia.Records
            };
            return registro;
        }

        public void registrarEstadoDeAsistencia(TypeAsistencia asistencia, TypeResultOracle resultOracle)
        {

           
            asistencia.RegSoa = resultOracle.o_estatus.Equals("OK") ? "SI" : "NO";

            Log("registrando estatus de asistencia "+ resultOracle.o_estatus);

            _repositorioAsistencia.registrarEstadoDeAsistencia(asistencia, resultOracle);

            Log("Se inserto correctamente " + resultOracle.o_estatus);
        }

        public void Log(string mensaje)
        {
            try
            {
        //        string ruta = Path.Combine(
        //Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        //"Log.txt");

                //string ruta = Directory.GetCurrentDirectory();
                string ruta ="C:\\inetpub\\wwwroot\\biostar\\Log.txt";

                if (!File.Exists(ruta))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(ruta))
                    {
                        sw.WriteLine(mensaje + DateTime.Now.ToString());

                    }
                }

                using (StreamWriter file = new StreamWriter(ruta, true))
                {
                    file.WriteLine(mensaje); //se agrega información al documento
                    file.Close();
                }


            }
            catch (Exception)
            {

                throw;
            }

        }

      
    }
}
