using AlpuraBiostar.Datos.Asistencia;
using AlpuraBiostar.Datos.Types;
using AlpuraBiostar.Datos.Utilidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private static string _conexionWSAlpura;

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


        public bool syncRegistroOracle(List<TypeAsistencia> lstasistencias, bool logConsole = false)
        {

            try
            {
                int contador = 1;
                int total = lstasistencias.Count;


                Log("Iniciando el envio de " + total.ToString(), logConsole);

                foreach (var registro in lstasistencias)
                {
                    registro.Fecha = new Validaciones().validarFecha(registro.Fecha);
                    registro.Records = new Validaciones().validarFecha(registro.Records);

                    var registroAEnviar = crearRegistro(registro);

                    var json = JsonConvert.SerializeObject(registroAEnviar);

                    var a = GetJsonAsync(contador, total, json, registro, logConsole).Result;

                    contador++;
                }
                return true;
            }
            catch (Exception EX)
            {

                return false;
            }
        }

        public Task<TypeResultOracle> GetJsonAsync(int contador, int total, string json, TypeAsistencia registro, bool logConsole = false)
        {
            StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
            TypeResultOracle r = new TypeResultOracle();

            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 1, 0);
                client.BaseAddress = new Uri(_conexionWSAlpura);

                Log("registro  " + contador.ToString() + " / " + total + " -> " + json, logConsole);

                var response = client.PostAsync("biostar_sirhal/", payload).Result;

                if (response.IsSuccessStatusCode)
                {
                    //Log("registro  " + contador.ToString() + " / " + total + " -> Se obtuvo respuesta de WS");

                    try
                    {
                        var responseContent = response.Content;
                        string responseString = responseContent.ReadAsStringAsync().Result;

                        var resultadoOracle = JsonConvert.DeserializeObject<TypeResultOracle>(responseString);

                        r = resultadoOracle;
                        Log("registro  " + contador.ToString() + " / " + total + " -> Se desarializo correctamente -> " + responseString, logConsole);

                        registrarEstadoDeAsistencia(registro, resultadoOracle);
                    }
                    catch (Exception)
                    {
                        var responseContent = response.Content;
                        string responseString = responseContent.ReadAsStringAsync().Result;
                        // Log("registro  " + contador.ToString() + " / " + total + json + "-> Se desarializo segunda forma");
                        var resultadoOracle = JsonConvert.DeserializeObject<TypeResultOracleVariable>(responseString);
                        Log("registro  " + contador.ToString() + " / " + total + " -> Se desarializo segunda forma correcta -> " + responseString, logConsole);

                        var res = new TypeResultOracle() { o_estatus = resultadoOracle.o_estatus.nil, o_descripcion = resultadoOracle.o_descripcion.nil };
                        registrarEstadoDeAsistencia(registro, res, logConsole);
                    }
                }
                else
                {
                    var res = new TypeResultOracle() { o_estatus = "NO", o_descripcion = "No hay Conexion con el WS" };

                    Log("registro  " + contador.ToString() + " / " + total + " -> NO Se obtuvo respuesta de WS !!!! :(", logConsole);
                    registrarEstadoDeAsistencia(registro, res, logConsole);
                }


            }
            //return r
            return Task.FromResult(r);
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

        public void registrarEstadoDeAsistencia(TypeAsistencia asistencia, TypeResultOracle resultOracle, bool logConsole = false)
        {
            if (resultOracle.o_estatus.Equals("OK"))
            {
                asistencia.RegSoa = "SI";
            }
            else if (resultOracle.o_estatus.Equals("true"))
            {
                asistencia.RegSoa = "SI";
            }
            else
            {
                asistencia.RegSoa = "NO";
            }
            //asistencia.RegSoa = resultOracle.o_estatus.Equals("OK") ? "SI" : "NO";
            //Log("registrando estatus de asistencia " + resultOracle.o_estatus);
            _repositorioAsistencia.registrarEstadoDeAsistencia(asistencia, resultOracle);
            Log("Se inserto correctamente ", logConsole);
        }

        public static void Log(string mensaje, bool logConsole = false)
        {
            try
            {
                if (!logConsole)
                {
                    //string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Log.txt");
                    string ruta = "C:\\inetpub\\wwwroot\\biostar\\Log.txt";

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
                else
                {

                    Console.WriteLine(DateTime.Now.ToString() + " -> " + mensaje);
                }

            }
            catch (Exception)
            {


            }

        }


    }
}
