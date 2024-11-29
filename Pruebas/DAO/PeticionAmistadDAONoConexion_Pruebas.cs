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


        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            // Arrange
            // Act
            bool resultado = PeticionAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }


        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarTrue()
        {
            // Arrange

            // Act
            bool resultado = PeticionAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando no hay conexión a la base de datos debido a la captura de excepciones.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHayConexion_DeberiaRetornarListaVacia()
        {
            // Arrange

            // Act
            var resultado = PeticionAmistadDAO.ObtenerSolicitudesAmistad(ID_REMITENTE);

            // Assert
            Assert.IsNotNull(resultado, "El método debería retornar una lista, aunque sea vacía.");
            Assert.AreEqual(0, resultado.Count, "El método debería retornar una lista vacía cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            // Arrange

            // Act
            bool resultado = PeticionAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoNoHayConexion_DeberiaRetornarFalse()
        {
            // Arrange

            // Act
            bool resultado = PeticionAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando no hay conexión a la base de datos.");
        }

    }
}
