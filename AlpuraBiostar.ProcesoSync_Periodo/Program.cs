using AlpuraBiostar.Datos.Utilidades;
using AlpuraBiostar.Negocio.Asistencia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpuraBiostar.ProcesoSync_Periodo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var appLog = new EventLog("Application");
            //appLog.Source = "AlpuraBiostar.ProcesoSync_Periodo";
            //lappLog.WriteEntry(DateTime.Now.ToString() + " -> Inicio de la Sincronizacion por periodos");
            ejecutarProcesoSync();
            //appLog.WriteEntry(DateTime.Now.ToString() + " -> Fin de la Sincronizacion por periodos");
        }


        public static void ejecutarProcesoSync()
        {
            Console.WriteLine(DateTime.Now.ToString() + " -> Inicio de la Sincronizacion");

            string conexionMariaDB = ConfigurationManager.AppSettings["conexionMariaDB"].ToString();
            string conexionWSAlpura = ConfigurationManager.AppSettings["conexionWSAlpura"].ToString();

            //string conexionMariaDB = "datasource=172.108.17.3;port=3312;username=alpura;password=Alpura#2018;database=biostar_tna;";
            //string conexionWSAlpura = "http://soa.alpura.com:17005/ALP_RH_CHECADORES_SB/BiostarSirhalRS/";

            syncRegistros(conexionMariaDB, conexionWSAlpura);

            Console.WriteLine(DateTime.Now.ToString() + " -> Fin de la Sincronizacion");
        }

        public static void syncRegistros(string conexionMariaDB, string conexionWSAlpura)
        {
            var LogTxt = Convert.ToBoolean(ConfigurationManager.AppSettings["LogTxt"].ToString());
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
                
                _negocioAsistencia.syncRegistroOracle(registrosNoSync, LogTxt);
            }


        }



    }
}
