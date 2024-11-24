using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioInvitacionPartidaPruebas
    {
        private Mock<IContextoOperacion> mockContextoOperacion;
        private ManejadorPrincipal manejador;
        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoOperacion = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoOperacion.Object);
        }

        [TestMethod]
        public void EnviarInvitacion_UsuarioConectado_DeberiaEnviarExitosamente()
        {
            // Arrange
            var callbackInvitacionMock = new Mock<IInvitacionPartidaCallback>();

            var receptor = new Usuario
            {
                IdUsuario = 1,
                Nombre = "ReceptorPrueba",
                InvitacionPartidaCallBack = callbackInvitacionMock.Object
            };

            var gamertagEmisor = "EmisorPrueba";
            var codigoSala = "123456";
            var gamertagReceptor = "ReceptorPrueba";

            // Act
            _ = manejador.ConectarUsuario(receptor);
            var resultado = manejador.EnviarInvitacion(gamertagEmisor, codigoSala, gamertagReceptor);

            // Assert
            Assert.IsTrue(resultado, "La invitación debería haberse enviado correctamente.");
            callbackInvitacionMock.Verify(c => c.RecibirInvitacion(It.IsAny<InvitacionPartida>()), Times.Once, "El callback debería haber sido llamado exactamente una vez.");
            manejador.DesconectarUsuario(receptor.IdUsuario);
        }


        [TestMethod]
        public void EnviarInvitacion_UsuarioNoConectado_DeberiaFallar()
        {
            // Arrange
            var gamertagEmisor = "EmisorPrueba";
            var codigoSala = "Sala123";
            var gamertagReceptor = "ReceptorNoConectado";

            // Act
            var resultado = manejador.EnviarInvitacion(gamertagEmisor, codigoSala, gamertagReceptor);

            // Assert
            Assert.IsFalse(resultado, "La invitación no debería haberse enviado porque el receptor no está conectado.");
        }
    }
}
