using AlpuraBiostar.Datos.Asistencia;
using AlpuraBiostar.Datos.Types;
using AlpuraBiostar.Datos.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
            catch (Exception)
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

                foreach (var registro in lstasistencias)
                {

                    registro.Fecha = new Validaciones().validarFecha(registro.Fecha);
                    registro.Records = new Validaciones().validarFecha(registro.Records);

                    var registroAEnviar = crearRegistro(registro);

                    var json = JsonConvert.SerializeObject(registroAEnviar);
                    StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var client = new HttpClient())
                    {
                        client.Timeout = new TimeSpan(0, 1, 0);
                        client.BaseAddress = new Uri(_conexionWSAlpura);
                        var response = client.PostAsync("biostar_sirhal/", payload).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = response.Content;
                            string responseString = responseContent.ReadAsStringAsync().Result;
                            var resultadoOracle = JsonConvert.DeserializeObject<TypeResultOracle>(responseString);

                            registrarEstadoDeAsistencia(registro, resultadoOracle);
                        }
                    }
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

            _repositorioAsistencia.registrarEstadoDeAsistencia(asistencia, resultOracle);

        }

    }
}
