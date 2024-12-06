using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioInvitacionPartidaPruebas : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        [TestMethod]
        public void EnviarInvitacion_UsuarioConectado_DeberiaEnviarExitosamente()
        {
            var receptor = new Usuario
            {
                IdUsuario = 1,
                Nombre = "unaay123",
                UsuarioSesionCallback = implementacionCallback
            };

            var gamertagEmisor = "EmisorPrueba";
            var codigoSala = "123456";
            var gamertagReceptor = receptor.Nombre;

            _ = manejador.ConectarUsuario(receptor);

            var resultado = manejador.EnviarInvitacion(new InvitacionPartida(gamertagEmisor, codigoSala, gamertagReceptor));

            Assert.IsTrue(resultado, "La invitación debería haberse enviado correctamente.");
            Assert.IsTrue(implementacionCallback.InvitacionEnviada, "El callback debería haber sido llamado exactamente una vez.");
        }


        [TestMethod]
        public void EnviarInvitacion_UsuarioNoConectado_DeberiaFallar()
        {
            var gamertagEmisor = "EmisorPrueba";
            var codigoSala = "Sala123";
            var gamertagReceptor = "ReceptorNoConectado";

            var resultado = manejador.EnviarInvitacion(new InvitacionPartida(gamertagEmisor, codigoSala, gamertagReceptor));

            Assert.IsFalse(resultado, "La invitación debería haber fallado al enviarse.");
            Assert.IsFalse(implementacionCallback.InvitacionEnviada, "El callback debería haber sido llamado exactamente cero veces.");
        }
    }
}
