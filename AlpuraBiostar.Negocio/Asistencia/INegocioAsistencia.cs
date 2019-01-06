using AlpuraBiostar.Datos.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Negocio.Asistencia
{
    public interface INegocioAsistencia
    {
        bool  syncRegistros();

        List<TypeAsistencia> obtenerRegistrosPorRangoDeFechas(string fechaInicio, string fechaFin);

        bool syncRegistroOracle(List<TypeAsistencia> lstasistencias);

        void registrarEstadoDeAsistencia(TypeAsistencia asistencia, TypeResultOracle resultOracle);
    }
}
