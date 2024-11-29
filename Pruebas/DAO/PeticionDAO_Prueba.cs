using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class SolicitudAmistadDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int ID_REMITENTE = 1;
        private const int ID_DESTINATARIO = 2;
        private const string ESTADO_PENDIENTE = "Pendiente";

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoUsuariosValidos_RetornaTrue()
        {
            // Arrange: Preparar los IDs de usuarios válidos
            int remitente = ID_REMITENTE;
            int destinatario = ID_DESTINATARIO;

            // Act: Intentar guardar la solicitud de amistad
            var result = SolicitudAmistadDAO.GuardarSolicitudAmistad(remitente, destinatario);

            // Assert: Verificar que la operación fue exitosa
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad es válida y se guarda correctamente.");
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoMismoUsuario_RetornaFalse()
        {
            // Arrange: Usar el mismo ID para remitente y destinatario
            int usuario = ID_REMITENTE;

            // Act: Intentar guardar la solicitud de amistad
            var result = SolicitudAmistadDAO.GuardarSolicitudAmistad(usuario, usuario);

            // Assert: Verificar que la operación falla
            Assert.IsFalse(result, "Debería retornar false si los IDs del remitente y destinatario son iguales.");
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoYaHaySolicitud_TiraExcepcion()
        {
            // Arrange: Configurar una solicitud de amistad entre dos usuarios
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            // Act & Assert
            Assert.ThrowsException<SolicitudAmistadExcepcion>(() =>
            {
                SolicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            });
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoYaSonAmigos_TiraExcepcion()
        {
            // Arrange: Configurar una solicitud de amistad entre dos usuarios
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            SolicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            // Act & Assert
            Assert.ThrowsException<SolicitudAmistadExcepcion>(() =>
            {
                SolicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            });
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoExisteSolicitud_RetornaTrue()
        {
            // Arrange: Configurar una solicitud de amistad entre dos usuarios
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            // Act: Verificar si la solicitud existe
            var result = SolicitudAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que la solicitud existe
            Assert.IsTrue(result, "Debería retornar true si existe una solicitud de amistad entre los usuarios.");
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoExisteSolicitud_RetornaFalse()
        {
            // Act: Verificar si existe una solicitud sin configurarla previamente
            var result = SolicitudAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que no hay ninguna solicitud
            Assert.IsFalse(result, "Debería retornar false si no existe ninguna solicitud de amistad entre los usuarios.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            // Arrange: Configurar una solicitud de amistad
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            // Act: Intentar aceptar la solicitud
            var result = SolicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que la solicitud fue aceptada
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se acepta correctamente.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            // Act: Intentar aceptar una solicitud inexistente
            var result = SolicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que la operación falla
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para aceptar.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            // Arrange: Configurar una solicitud de amistad
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            // Act: Intentar rechazar la solicitud
            var result = SolicitudAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que la solicitud fue rechazada
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se rechaza correctamente.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            // Act: Intentar rechazar una solicitud inexistente
            var result = SolicitudAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);

            // Assert: Verificar que la operación falla
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para rechazar.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoHaySolicitudes_RetornaListaUsuarios()
        {
            // Arrange: Configurar una solicitud de amistad
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            // Act: Obtener las solicitudes de amistad para un usuario
            var result = SolicitudAmistadDAO.ObtenerSolicitudesAmistad(ID_DESTINATARIO);

            // Assert: Verificar que se obtiene una lista con al menos una solicitud
            Assert.IsNotNull(result, "Debería retornar una lista de usuarios.");
            Assert.IsTrue(result.Count > 0, "Debería retornar una lista con al menos una solicitud.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHaySolicitudes_RetornaListaVacia()
        {
            // Act: Obtener las solicitudes de amistad sin configurarlas previamente
            var result = SolicitudAmistadDAO.ObtenerSolicitudesAmistad(ID_REMITENTE);

            // Assert: Verificar que se obtiene una lista vacía
            Assert.IsNotNull(result, "Debería retornar una lista.");
            Assert.AreEqual(0, result.Count, "Debería retornar una lista vacía si no hay solicitudes.");
        }

        private static void ConfigurarSolicitud(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = new SolicitudAmistad
                    {
                        idRemitente = idRemitente,
                        idDestinatario = idDestinatario,
                        fechaSolicitud = DateTime.Now,
                        estado = ESTADO_PENDIENTE
                    };
                    context.SolicitudAmistad.Add(solicitud);
                    context.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        Console.WriteLine($"Propiedad: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                throw; // Vuelve a lanzar la excepción para que no se silencie.
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = new DescribeloEntities())
            {
                var solicitudes = context.SolicitudAmistad
                    .Where(solicitud => (solicitud.idRemitente == ID_REMITENTE && solicitud.idDestinatario == ID_DESTINATARIO) ||
                                (solicitud.idRemitente == ID_DESTINATARIO && solicitud.idDestinatario == ID_REMITENTE))
                    .ToList();

                if (solicitudes.Any())
                {
                    context.SolicitudAmistad.RemoveRange(solicitudes);
                    context.SaveChanges();
                }
            }
        }
    }
}
