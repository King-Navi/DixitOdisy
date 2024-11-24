using DAOLibreria.DAO;
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
    public class PeticionAmistadDAO_Prueba : ConfiguracionPruebaBD
    {
        private int idUsuario1;
        private int idUsuario2;

        [TestInitialize]
        public void Setup()
        {
            idUsuario1 = 1;
            idUsuario2 = 2;
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoUsuariosValidos_RetornaTrue()
        {
            // Act
            var result = PeticionAmistadDAO.GuardarSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoMismoUsuario_RetornaFalse()
        {
            // Arrange
            int idUsuario1 = 1;
            int idUsuario2 = 1;

            // Act
            var result = PeticionAmistadDAO.GuardarSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoErrorEnBaseDatos_RetornaFalse()
        {
            // Arrange
            int idUsuario1 = 1;
            int idUsuario2 = 9999999; // ID que causa un error intencionado (por ejemplo, no existe)

            // Act
            var result = PeticionAmistadDAO.GuardarSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoExisteSolicitud_RetornaTrue()
        {
            // Arrange
            int idUsuario1 = 1;
            int idUsuario2 = 2;

            // Asegurarse de que existe una solicitud entre estos usuarios en la base de datos.

            // Act
            var result = PeticionAmistadDAO.ExisteSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoExisteSolicitud_RetornaFalse()
        {
            // Arrange
            int idUsuario1 = 1;
            int idUsuario2 = 3;

            // Act
            var result = PeticionAmistadDAO.ExisteSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoErrorEnBaseDatos_RetornaTrue()
        {
            // Arrange
            int idUsuario1 = 999999; // ID que causa error
            int idUsuario2 = 888888; // ID que causa error

            // Act
            var result = PeticionAmistadDAO.ExisteSolicitudAmistad(idUsuario1, idUsuario2);

            // Assert
            Assert.IsTrue(result); // Dado el código actual, el método retorna true en caso de excepción.
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoHaySolicitudes_RetornaListaUsuarios()
        {
            // Arrange
            int idUsuario = 2;

            // Act
            var result = PeticionAmistadDAO.ObtenerSolicitudesAmistad(idUsuario);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0); // Asegúrate de que haya solicitudes pendientes para este usuario en la base de datos de prueba.
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHaySolicitudes_RetornaListaVacia()
        {
            // Arrange
            int idUsuario = 5; // ID sin solicitudes pendientes

            // Act
            var result = PeticionAmistadDAO.ObtenerSolicitudesAmistad(idUsuario);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoErrorEnBaseDatos_RetornaListaVacia()
        {
            // Arrange
            int idUsuario = 999999; // ID que causa error

            // Act
            var result = PeticionAmistadDAO.ObtenerSolicitudesAmistad(idUsuario);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            // Arrange
            int idRemitente = 1;
            int idDestinatario = 2;

            // Act
            var result = PeticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            // Arrange
            int idRemitente = 3;
            int idDestinatario = 4;

            // Act
            var result = PeticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoErrorEnBaseDatos_RetornaFalse()
        {
            // Arrange
            int idRemitente = 999999; // ID que causa error
            int idDestinatario = 888888; // ID que causa error

            // Act
            var result = PeticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            // Arrange
            int idRemitente = 1;
            int idDestinatario = 2;

            // Act
            var result = PeticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            // Arrange
            int idRemitente = 3;
            int idDestinatario = 4;

            // Act
            var result = PeticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoErrorEnBaseDatos_RetornaFalse()
        {
            // Arrange
            int idRemitente = 999999; // ID que causa error
            int idDestinatario = 888888; // ID que causa error

            // Act
            var result = PeticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Eliminar la solicitud de amistad creada
            using (var context = new DescribeloEntities())
            {
                var solicitud = context.PeticionAmistad
                    .FirstOrDefault(p => p.idRemitente == idUsuario1 && p.idDestinatario == idUsuario2);

                if (solicitud != null)
                {
                    context.PeticionAmistad.Remove(solicitud);
                    context.SaveChanges();
                }
            }
        }

    }

}


