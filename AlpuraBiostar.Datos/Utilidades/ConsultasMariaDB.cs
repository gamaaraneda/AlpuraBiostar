using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.Datos.Utilidades
{
    public class ConsultasMariaDB
    {

        private string _conexionMariaDB;

        public ConsultasMariaDB(string conexionMariaDB)
        {
            _conexionMariaDB = conexionMariaDB;
        }

        public DataTable ejecutarStore(string nombreStore, Dictionary<string, string> parametros=null)
        {
            
            // Prepara la conexión
            MySqlConnection databaseConnection = new MySqlConnection(_conexionMariaDB);
            DataTable dtResultado = new DataTable();

            try
            {
                // abrimos la conexion a la base de datos
                databaseConnection.Open();

                // Indicamos que estore vamos a usar
                MySqlCommand commandDatabase = new MySqlCommand(nombreStore, databaseConnection);
                // Asignamos el tipo de consulta que sera
                commandDatabase.CommandType = CommandType.StoredProcedure;
                commandDatabase.CommandTimeout = 60;
                MySqlDataReader reader;

                //// Mandamos los parametros al store
                if (parametros != null && parametros.Count > 0)
                {
                    foreach (var parametro in parametros)
                    {
                        commandDatabase.Parameters.AddWithValue(parametro.Key, parametro.Value);
                    }

                }

                // Ejecutamos el store
                reader = commandDatabase.ExecuteReader();

                // Los datos del store se almacenan en el datatable
                dtResultado.Load(reader);
                reader.Close();

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("Error al ejecutar Consulta BD: " + ex);
            }
            finally
            {
                databaseConnection.Close();
            }


            return dtResultado;

        }

    }
}
