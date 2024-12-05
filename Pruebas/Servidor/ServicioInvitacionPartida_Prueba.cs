using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using WcfServicioLibreria.Contratos;
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

        //[TestMethod]
        //public void EnviarInvitacion_UsuarioConectado_DeberiaEnviarExitosamente()
        //{

        //    var callbackInvitacionMock = new Mock<IUsuarioSesionCallback>();

        //    var receptor = new Usuario
        //    {
        //        IdUsuario = 1,
        //        Nombre = "ReceptorPrueba",
        //    };

        //    var gamertagEmisor = "EmisorPrueba";
        //    var codigoSala = "123456";
        //    var gamertagReceptor = "ReceptorPrueba";


        //    _ = manejador.ConectarUsuario(receptor);
        //    var resultado = manejador.EnviarInvitacion(gamertagEmisor, codigoSala, gamertagReceptor);


        //    Assert.IsTrue(resultado, "La invitación debería haberse enviado correctamente.");
        //    callbackInvitacionMock.Verify(c => c.RecibirInvitacionCallback(It.IsAny<InvitacionPartida>()), Times.Once, "El callback debería haber sido llamado exactamente una vez.");
        //    manejador.DesconectarUsuario(receptor.IdUsuario);
        //}


        //[TestMethod]
        //public void EnviarInvitacion_UsuarioNoConectado_DeberiaFallar()
        //{

        //    var gamertagEmisor = "EmisorPrueba";
        //    var codigoSala = "Sala123";
        //    var gamertagReceptor = "ReceptorNoConectado";


        //    var resultado = manejador.EnviarInvitacion(gamertagEmisor, codigoSala, gamertagReceptor);


        //    Assert.IsFalse(resultado, "La invitación no debería haberse enviado porque el receptor no está conectado.");
        //}
    }
}
