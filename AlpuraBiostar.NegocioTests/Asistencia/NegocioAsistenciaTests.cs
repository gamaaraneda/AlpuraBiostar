using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlpuraBiostar.Negocio.Asistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlpuraBiostar.Datos.Types;

namespace AlpuraBiostar.Negocio.Asistencia.Tests
{
    [TestClass()]
    public class NegocioAsistenciaTests
    {
        [TestMethod()]
        public void registrarEstadoDeAsistenciaTest()
        {
            INegocioAsistencia negocioAsistencia = new NegocioAsistencia("datasource=172.108.17.8;port=3312;username=alpura;password=Alpura#2018;database=biostar_tna;", "http://soats.alpura.com:17005/ALP_RH_CHECADORES_SB/BiostarSirhalRS/");

            var asistencia = new TypeAsistencia() {
                Fecha= "2018-12-10 15:51:54",
                EmployeNumber= "121612",
                RegSoa="SI"
            };

            var resutoracle = new TypeResultOracle() { o_estatus="ERROR",o_descripcion="NO SE ENCUENTR ID"};

            negocioAsistencia.registrarEstadoDeAsistencia(asistencia,resutoracle);

            Assert.Fail();
        }
    }
}