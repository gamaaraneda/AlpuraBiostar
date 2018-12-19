using AlpuraBiostar.Datos.Asistencia;
using AlpuraBiostar.Datos.Types;
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

        public async Task<bool> syncRegistros()
        {
            try
            {
                var listRegistros = _repositorioAsistencia.obtenerRegistrosNoSync();
                var result = await syncRegistroOracle(listRegistros);
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


        public async Task<bool> syncRegistroOracle(List<TypeAsistencia> lstasistencias)
        {
            try
            {
                var httpClient = new HttpClient()
                {
                    Timeout = new TimeSpan(0, 1, 0),
                    BaseAddress = new Uri(_conexionWSAlpura)
                };

                foreach (var registro in lstasistencias)
                {
                    var registroAEnviar = crearRegistro(registro);

                    var json = JsonConvert.SerializeObject(registroAEnviar);
                    StringContent payload = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = httpClient.PostAsync("biostar_sirhal/", payload);
                    var content = await result.Result.Content.ReadAsStringAsync();
                    var resultadoOracle = JsonConvert.DeserializeObject<TypeResultOracle>(content);

                    registrarEstadoDeAsistencia(registro, resultadoOracle);

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
