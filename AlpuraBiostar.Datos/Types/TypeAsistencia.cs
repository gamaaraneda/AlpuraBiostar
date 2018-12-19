using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Types
{
    public class TypeAsistencia
    {
        [JsonProperty(PropertyName = "p_employee_number")]
        public string EmployeNumber { get; set; }
        [JsonProperty(PropertyName = "p_fecha")]
        public string Fecha { get; set; }
        [JsonProperty(PropertyName = "p_ubicacion")]
        public string Ubicacion { get; set; }
        [JsonProperty(PropertyName = "p_hostname")]
        public string Hostname { get; set; }
        public string RegSoa { get; set; }
        public string  Status { get; set; }
        public string Descripcion { get; set; }
        [JsonProperty(PropertyName = "p_record")]
        public string Records { get; set; }

    }
}
