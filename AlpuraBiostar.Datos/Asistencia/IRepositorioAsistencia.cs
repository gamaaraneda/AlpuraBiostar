using AlpuraBiostar.Datos.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Asistencia
{
    public interface IRepositorioAsistencia
    {
        List<TypeAsistencia> obtenerRegistrosNoSync();
        List<TypeAsistencia> obtenerRegistrosPorRangoDeFechas(string fechaInicio,string fechaFin);
    }
}
