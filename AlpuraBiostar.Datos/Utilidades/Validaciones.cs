using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Utilidades
{
    public class Validaciones
    {
        public int validarEnteros(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return 0;
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return int.Parse(valor);
        }

        public float validarFlotantes(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return 0;
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return float.Parse(valor);
        }

        public double validarDoubles(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return 0;
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return double.Parse(valor);
        }

        public bool validarBoleanos(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return false;
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return bool.Parse(valor);

        }

        public int ConvertirBooleanos(bool valor)
        {
            var a = valor == true ? 1 : 0;

            return a;

        }

        public string validarCadenasNulas(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return valor.ToString();

        }

        public string validarFecha(string fecha)
        {
            DateTimeFormatInfo mxCulture = new CultureInfo("es-MX").DateTimeFormat;
            var fechaCorrecta = Convert.ToDateTime(fecha, mxCulture).ToString("yyyy-MM-dd HH:mm:ss");

            return fechaCorrecta;
        }

    }
}
