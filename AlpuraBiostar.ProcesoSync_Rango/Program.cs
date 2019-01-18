using AlpuraBiostar.Datos.Utilidades;
using AlpuraBiostar.Negocio.Asistencia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlpuraBiostar.ProcesoSync_Rango
{
    class Program
    {
        static void Main(string[] args)
        {
            
           
            ejecutarProcesoSync();
            Console.ReadLine();
        }

        public static void ejecutarProcesoSync()
        {
            Console.WriteLine(DateTime.Now.ToString() + " -> Inicio de la Sincronizacion");

            string conexionMariaDB = ConfigurationManager.AppSettings["conexionMariaDB"].ToString();
            string conexionWSAlpura = ConfigurationManager.AppSettings["conexionWSAlpura"].ToString();

            syncRegistros(conexionMariaDB, conexionWSAlpura);

            Console.WriteLine(DateTime.Now.ToString() + " -> Fin de la Sincronizacion");
        }

        public static void syncRegistros(string conexionMariaDB, string conexionWSAlpura)
        {
            
            var FechaInicio = ConfigurationManager.AppSettings["FechaInicial"];
            var FechaFin = ConfigurationManager.AppSettings["FechaIFinal"];

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
