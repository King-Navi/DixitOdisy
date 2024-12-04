using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioSalaJugador_Prueba : ConfiguradorPruebaParaServicio
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
        #region AgregarJugadorSala
        #endregion AgregarJugadorSala
        #region ComenzarPartidaAnfrition
        #endregion ComenzarPartidaAnfrition
        #region
        #endregion

    }
}
