using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class PeticionAmistadDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int ID_REMITENTE = 1;
        private const int ID_DESTINATARIO = 2;

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoUsuariosValidos_RetornaTrue()
        {
            var result = PeticionAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad es válida y se guarda correctamente.");
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoMismoUsuario_RetornaFalse()
        {
            var result = PeticionAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_REMITENTE);
            Assert.IsFalse(result, "Debería retornar false si los IDs del remitente y destinatario son iguales.");
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoExisteSolicitud_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = PeticionAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si existe una solicitud de amistad entre los usuarios.");
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoExisteSolicitud_RetornaFalse()
        {
            var result = PeticionAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe ninguna solicitud de amistad entre los usuarios.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = PeticionAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se acepta correctamente.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            var result = PeticionAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para aceptar.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = PeticionAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se rechaza correctamente.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            var result = PeticionAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para rechazar.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoHaySolicitudes_RetornaListaUsuarios()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = PeticionAmistadDAO.ObtenerSolicitudesAmistad(ID_DESTINATARIO);
            Assert.IsNotNull(result, "Debería retornar una lista de usuarios.");
            Assert.IsTrue(result.Count > 0, "Debería retornar una lista con al menos una solicitud.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHaySolicitudes_RetornaListaVacia()
        {
            var result = PeticionAmistadDAO.ObtenerSolicitudesAmistad(ID_DESTINATARIO);
            Assert.IsNotNull(result, "Debería retornar una lista.");
            Assert.AreEqual(0, result.Count, "Debería retornar una lista vacía si no hay solicitudes.");
        }

        private static void ConfigurarSolicitud(int idRemitente, int idDestinatario)
        {
            using (var context = new DescribeloEntities())
            {
                var solicitud = new PeticionAmistad
                {
                    idRemitente = idRemitente,
                    idDestinatario = idDestinatario,
                    fechaPeticion = DateTime.Now
                };
                context.PeticionAmistad.Add(solicitud);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = new DescribeloEntities())
            {
                var solicitudes = context.PeticionAmistad
                    .Where(p => (p.idRemitente == ID_REMITENTE && p.idDestinatario == ID_DESTINATARIO) ||
                                (p.idRemitente == ID_DESTINATARIO && p.idDestinatario == ID_REMITENTE))
                    .ToList();

                if (solicitudes.Any())
                {
                    context.PeticionAmistad.RemoveRange(solicitudes);
                    context.SaveChanges();
                }
            }
        }
    }
}
