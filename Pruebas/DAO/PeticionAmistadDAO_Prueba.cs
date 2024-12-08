using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class SolicitudAmistadDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int ID_REMITENTE = 1;
        private const int ID_DESTINATARIO = 2;
        private const string ESTADO_PENDIENTE = "Pendiente";
        private SolicitudAmistadDAO solicitudAmistadDAO = new SolicitudAmistadDAO();


        [TestInitialize]
        public void CleanupSolicitudesAmistad()
        {
            using (var context = new DescribeloEntities())
            {
                var ids = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                var solicitudesParaEliminar = context.Amigo
                    .Where(solicitud =>
                        ids.Contains(solicitud.idMayor_usuario) || ids.Contains(solicitud.idMenor_usuario))
                    .ToList();
                context.Amigo.RemoveRange(solicitudesParaEliminar);
                context.SaveChanges();
            }
        }


        #region GuardarSolicitudAmistad
        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoUsuariosValidos_RetornaTrue()
        {
            var result = solicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad es válida y se guarda correctamente.");
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoMismoUsuario_RetornaFalse()
        {
            var result = solicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_REMITENTE);
            Assert.IsFalse(result, "Debería retornar false si los IDs del remitente y destinatario son iguales.");
        }
        #endregion

        #region ExisteSolicitudAmistad
        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoYaHaySolicitud_TiraExcepcion()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);

            Assert.ThrowsException<SolicitudAmistadExcepcion>(() =>
            {
                solicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            });
        }

        [TestMethod]
        public void GuardarSolicitudAmistad_CuandoYaSonAmigos_TiraExcepcion()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            solicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.ThrowsException<SolicitudAmistadExcepcion>(() =>
            {
                solicitudAmistadDAO.GuardarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            });
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoExisteSolicitud_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = solicitudAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si existe una solicitud de amistad entre los usuarios.");
        }

        [TestMethod]
        public void ExisteSolicitudAmistad_CuandoNoExisteSolicitud_RetornaFalse()
        {
            var result = solicitudAmistadDAO.ExisteSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe ninguna solicitud de amistad entre los usuarios.");
        }
        #endregion

        #region AceptarSolicitudAmistad
        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = solicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se acepta correctamente.");
        }

        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            var result = solicitudAmistadDAO.AceptarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para aceptar.");
        }
        #endregion

        #region RechazarSolicitudAmistad
        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudExiste_RetornaTrue()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = solicitudAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsTrue(result, "Debería retornar true si la solicitud de amistad existe y se rechaza correctamente.");
        }

        [TestMethod]
        public void RechazarSolicitudAmistad_CuandoSolicitudNoExiste_RetornaFalse()
        {
            var result = solicitudAmistadDAO.RechazarSolicitudAmistad(ID_REMITENTE, ID_DESTINATARIO);
            Assert.IsFalse(result, "Debería retornar false si no existe una solicitud de amistad para rechazar.");
        }
        #endregion

        #region ObtenerSolicitudesAmistad
        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoHaySolicitudes_RetornaListaUsuarios()
        {
            ConfigurarSolicitud(ID_REMITENTE, ID_DESTINATARIO);
            var result = solicitudAmistadDAO.ObtenerSolicitudesAmistad(ID_DESTINATARIO);
            Assert.IsTrue(result.Count > 0, "Debería retornar una lista con al menos una solicitud.");
        }

        [TestMethod]
        public void ObtenerSolicitudesAmistad_CuandoNoHaySolicitudes_RetornaListaVacia()
        {
            var result = solicitudAmistadDAO.ObtenerSolicitudesAmistad(ID_DESTINATARIO);
            Assert.AreEqual(0, result.Count, "Debería retornar una lista vacía si no hay solicitudes.");
        }
        #endregion

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
            catch (System.Data.Entity.Validation.DbEntityValidationException excepcion)
            {
                foreach (var validationError in excepcion.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        Console.WriteLine($"Propiedad: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }
                throw;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = new DescribeloEntities())
            {
                var ids = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                var solicitudesParaEliminar = context.SolicitudAmistad
                    .Where(solicitud =>
                        ids.Contains(solicitud.idRemitente) || ids.Contains(solicitud.idDestinatario))
                    .ToList();
                context.SolicitudAmistad.RemoveRange(solicitudesParaEliminar);
                context.SaveChanges();
            }
        }
    }
}
