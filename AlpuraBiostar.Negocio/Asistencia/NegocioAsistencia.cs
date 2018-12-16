using AlpuraBiostar.Datos.Asistencia;
using AlpuraBiostar.Datos.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Negocio.Asistencia
{
    public class NegocioAsistencia:INegocioAsistencia
    {
        IRepositorioAsistencia _repositorioAsistencia;
        public NegocioAsistencia(string conexionMariaDB)
        {

            _repositorioAsistencia = new RepositorioAsistencia(conexionMariaDB);
        }

        public List<TypeAsistencia> obtenerRegistrosNoSync()
        {
            return _repositorioAsistencia.obtenerRegistrosNoSync();
        }

        public List<TypeAsistencia> obtenerRegistrosPorRangoDeFechas(string fechaInicio, string fechaFin)
        {
            return _repositorioAsistencia.obtenerRegistrosPorRangoDeFechas(fechaInicio,fechaFin);
        }
    }
}
