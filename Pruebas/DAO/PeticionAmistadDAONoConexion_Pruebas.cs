using DAOLibreria.DAO;
using DAOLibreria;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class PeticionAmistadDAONoConexion_Pruebas :ConfiguracionPruebaBDInvalida
    {
        private const int ID_REMITENTE = 1;
        private const int ID_DESTINATARIO = 2;
        private PeticionAmistadDAO peticionAmistadDAO = new PeticionAmistadDAO();

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            
            
            bool resultado = peticionAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }


        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarTrue()
        {
            

            
            bool resultado = peticionAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            
            Assert.IsTrue(resultado, "El método debería retornar true cuando no hay conexión a la base de datos debido a la captura de excepciones.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHayConexion_DeberiaRetornarListaVacia()
        {
            

            
            var resultado = peticionAmistadDAO.ObtenerSolicitudesAmistad(ID_REMITENTE);

            
            Assert.IsNotNull(resultado, "El método debería retornar una lista, aunque sea vacía.");
            Assert.AreEqual(0, resultado.Count, "El método debería retornar una lista vacía cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            

            
            bool resultado = peticionAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            

            
            bool resultado = peticionAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

    }
}
