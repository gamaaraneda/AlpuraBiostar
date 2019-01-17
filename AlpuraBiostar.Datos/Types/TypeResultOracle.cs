using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Types
{
    public class TypeResultOracle
    {
        public string o_estatus { get; set; }
        public string o_descripcion { get; set; }
   
    }

    public class OStaus
    {
        [JsonProperty(PropertyName = "@nil")]
        public string nil { get; set; }
    }

    public class ODescripcion
    {
        [JsonProperty(PropertyName = "@nil")]
        public string nil { get; set; }
    }

    public class TypeResultOracleVariable
    {
        public OStaus o_estatus { get; set; }
        public ODescripcion o_descripcion { get; set; }
    }


}
