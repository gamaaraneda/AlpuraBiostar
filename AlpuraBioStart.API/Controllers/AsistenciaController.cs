using AlpuraBiostar.Datos.Types;
using AlpuraBiostar.Datos.Utilidades;
using AlpuraBiostar.Negocio.Asistencia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using System.Web.Mvc;
//using System.Web.Mvc;

namespace AlpuraBioStart.API.Controllers
{
    public class AsistenciaController : Controller
    {
        INegocioAsistencia _negocioAsistencia;
         
        public AsistenciaController()
        {
            string conexionMariaDB = ConfigurationManager.ConnectionStrings["conexionMariaDB"].ConnectionString;
            string conexionWSAlpura = ConfigurationManager.ConnectionStrings["conexionWSAlpura"].ConnectionString;

            _negocioAsistencia = new NegocioAsistencia(conexionMariaDB, conexionWSAlpura);
           
        }

        public ActionResult Index()
        {
            
            return View();
        }


        //public List<TypeAsistencia> RegistrosNoSync()
        //{
        //    var lstAsistenciaNoSync = _negocioAsistencia.obtenerRegistrosNoSync();
        //    return lstAsistenciaNoSync;
        //}

        //public List<TypeAsistencia> RegistrosFiltroFechas()
        //{
        //    var lstAsistencia = _negocioAsistencia.obtenerRegistrosPorRangoDeFechas("2018/11/02", "2018/12/11");
        //    return lstAsistencia;
        //}

        [HttpGet]
        public JsonResult RegistrosNoSync()
        {
            var lstAsistenciaNoSync = _negocioAsistencia.syncRegistros();
            return Json(new { productos = lstAsistenciaNoSync }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegistrosFiltroFechas(string FechaInicio,string FechaFin)
        {
            FechaInicio = new Validaciones().validarFecha(FechaInicio);
            FechaFin = new Validaciones().validarFecha(FechaFin);

            var aa = new List<TypeAsistencia>();
            aa.Add(new TypeAsistencia() { Descripcion = "", EmployeNumber = "675", Fecha="15/12/2018", Ubicacion="mexico", Hostname="GSA", RegSoa="SI", Status="", Records="15/12/2018" });
            aa.Add(new TypeAsistencia() { Descripcion = "", EmployeNumber = "676", Fecha="16/12/2018", Ubicacion="mexico", Hostname="GSA", RegSoa="SI", Status="", Records="16/12/2018" });

            //var lstAsistencia = _negocioAsistencia.obtenerRegistrosPorRangoDeFechas(FechaInicio, FechaFin);
            return Json(new { result = aa }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SynRegistrosGrid(List<TypeAsistencia> lsAsistencias)
        {

            var a =_negocioAsistencia.syncRegistroOracle(lsAsistencias);

            return Json(new { result = a }, JsonRequestBehavior.AllowGet);
        }


    }
}
