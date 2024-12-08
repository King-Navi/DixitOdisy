using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using DAOLibreria.Excepciones;

namespace Pruebas.DAO
{
    [TestClass]
    public class SolicitudAmistadDAONoConexion_Pruebas :ConfiguracionPruebaBDInvalida
    {
        private const int ID_REMITENTE = 1;
        private const int ID_DESTINATARIO = 2;
        private SolicitudAmistadDAO solicitudAmistadDAO = new SolicitudAmistadDAO();

        [TestMethod]
        [ExpectedException(typeof(SolicitudAmistadExcepcion))]
        public void GuardarSolicitudAmistad_CuandoNoHayConexion_DeberiaLanzarExcepcion()
        {
            solicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
        }


        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarTrue()
        {
            bool resultado = solicitudAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(resultado, "El método debería retornar true cuando no hay conexión a la base de datos debido a la captura de excepciones.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHayConexion_DeberiaRetornarListaVacia()
        {
            var resultado = solicitudAmistadDAO.ObtenerSolicitudesAmistad(ID_REMITENTE);
            Assert.AreEqual(0, resultado.Count, "El método debería retornar una lista vacía cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            bool resultado = solicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            bool resultado = solicitudAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

    }
}
