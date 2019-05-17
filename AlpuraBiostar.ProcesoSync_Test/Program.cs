using AlpuraBiostar.Datos.Utilidades;
using AlpuraBiostar.Negocio.Asistencia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlpuraBiostar.ProcesoSync_Test
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static bool syncronizando = false;

        static void Main(string[] args)
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

            Console.WriteLine("Iniciando el Proceso de Sync");

            var tiemposync = ConfigurationManager.AppSettings["minutosEjecucion"].ToString();

            Timer _timerProcesoSync = new System.Threading.Timer(
                         e => ejecutarProcesoSync(),
                                null,
                                TimeSpan.Zero,
                                TimeSpan.FromMinutes(Convert.ToInt32(tiemposync)));

            while (true)
            { }

        }

        public static void ejecutarProcesoSync()
        {
            if (!syncronizando)
            {
                syncronizando = true;
                try
                {
                    Console.WriteLine();
                    Console.WriteLine("**************************************************************");
                    Console.WriteLine("**************************************************************");
                    Console.WriteLine();
                    Console.WriteLine(DateTime.Now.ToString() + " -> Inicio de la Sincronizacion");
                }
                catch (Exception)
                {

                    Console.WriteLine(" -> Inicio de la Sincronizacion");
                }

                string conexionMariaDB = ConfigurationManager.AppSettings["conexionMariaDB"].ToString();
                string conexionWSAlpura = ConfigurationManager.AppSettings["conexionWSAlpura"].ToString();

                syncRegistros(conexionMariaDB, conexionWSAlpura);
                syncronizando = false;
                Console.WriteLine();
                Console.WriteLine(DateTime.Now.ToString() + " -> **********  Fin de la Sincronizacion    **********");
                Console.WriteLine();
            }
            else {
                Console.WriteLine();
                Console.WriteLine(DateTime.Now.ToString() + " -> **********  Ya existe un proceso actual de sync, esperemos a que termine     **********");
                Console.WriteLine();
            }
        }

        public static void syncRegistros(string conexionMariaDB, string conexionWSAlpura)
        {
            var minutosSyncRango = Convert.ToInt32(ConfigurationManager.AppSettings["minutosSyncRango"].ToString());

            var FechaInicio = DateTime.Now.AddMinutes(minutosSyncRango).ToString();
            var FechaFin = DateTime.Now.ToString();

            FechaInicio = new Validaciones().validarFecha(FechaInicio);
            FechaFin = new Validaciones().validarFecha(FechaFin);

            INegocioAsistencia _negocioAsistencia = new NegocioAsistencia(conexionMariaDB, conexionWSAlpura);

            var lstAsistencia = _negocioAsistencia.obtenerRegistrosPorRangoDeFechas(FechaInicio, FechaFin);
            //var lstAsistencia = _negocioAsistencia.obtenerRegistrosPorRangoDeFechas("2019-01-18 11:00:00", "2019-01-18 13:00:00");

            var registrosNoSync = lstAsistencia.Where(r => r.RegSoa.Equals("NO")).ToList();

            Console.WriteLine(DateTime.Now.ToString() + " -> Syncronizando " + registrosNoSync.Count.ToString() + " registros");

            if (registrosNoSync.Count > 0)
            {
                // Console.WriteLine(DateTime.Now.ToString() + " -> Syncronizando "+registrosNoSync.Count.ToString()+" registros");

                _negocioAsistencia.syncRegistroOracle(registrosNoSync, true);
            }


        }




    }
}
