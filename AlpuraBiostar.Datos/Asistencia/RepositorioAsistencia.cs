using AlpuraBiostar.Datos.Utilidades;
using AlpuraBiostar.Datos.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Asistencia
{
    public class RepositorioAsistencia : IRepositorioAsistencia
    {
        private ConsultasMariaDB _consultasMariaDB;
        private Validaciones _validaciones;
        public RepositorioAsistencia(string conexionMariaDB)
        {
            _consultasMariaDB = new ConsultasMariaDB(conexionMariaDB);
            _validaciones = new Validaciones();
        }

        public List<TypeAsistencia> obtenerRegistrosNoSync()
        {
            var dtAsistencia = _consultasMariaDB.ejecutarStore("Asistencia_obtenerRegistrosNoSync");
            var lstAsistencia = obtenerDatosAsistencias(dtAsistencia);
            return lstAsistencia;
        }

        public List<TypeAsistencia> obtenerRegistrosPorRangoDeFechas(string fechaInicio, string fechaFin)
        {
            var dicParametros = new Dictionary<string, string>() {
                { "@fechaInicio", fechaInicio },
                { "@fechaFin", fechaFin}
            };
            var dtAsistencia = _consultasMariaDB.ejecutarStore("Asistencia_obtenerRegistrosPorRangoDeFechas", dicParametros);
            var lstAsistencia = obtenerDatosAsistencias(dtAsistencia);

            return lstAsistencia;
        }


        #region Privados

        private List<TypeAsistencia> obtenerDatosAsistencias(DataTable dtAsistencias)
        {
            List<TypeAsistencia> lstAsistencias = new List<TypeAsistencia>();

            if (dtAsistencias != null)
            {
                if (dtAsistencias.Rows.Count > 0)
                {
                    lstAsistencias = (from DataRow dr in dtAsistencias.Rows
                                      select new TypeAsistencia()
                                      {
                                          EmployeNumber = _validaciones.validarCadenasNulas(dr["EMPLOYE_NUMBER"].ToString()),
                                          Fecha = _validaciones.validarCadenasNulas(dr["FECHA"].ToString()),
                                          Ubicacion = _validaciones.validarCadenasNulas(dr["UBICACION"].ToString()),
                                          Hostname = _validaciones.validarCadenasNulas(dr["HOSTNAME"].ToString()),
                                          RegSoa = _validaciones.validarCadenasNulas(dr["REG_SOA"].ToString()),
                                          Status = _validaciones.validarCadenasNulas(dr["STATUS"].ToString()),
                                          Descripcion = _validaciones.validarCadenasNulas(dr["DESCRIPCION"].ToString()),
                                          Records = _validaciones.validarCadenasNulas(dr["RECORDS"].ToString())
                                      }).ToList();

                }
            }
            return lstAsistencias;

        }

        #endregion



    }
}
