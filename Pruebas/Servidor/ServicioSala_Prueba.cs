﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioSala_Prueba : ConfiguradorPruebaParaServicio
    {
        private ConfiguracionPartida configuracionGenerica;

        [TestInitialize]
        protected override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<System.DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        protected override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }


        #region CrearSala
        [TestMethod]
        public void CrearSala_ParametroNull_RetornaFalse()
        {
            // Arrange
            string anfitrion = null;

            // Act
            string idSala = manejador.CrearSala(anfitrion);

            // Assert
            Assert.IsNull(idSala, "El idsala es null.");


        }
        [TestMethod]
        public void CrearSala_ParametroValido_RetornaFalse()
        {
            // Arrange
            string anfitrion = "Navi";

            // Act
            string idSala = manejador.CrearSala(anfitrion);

            // Assert
            Assert.IsNotNull(idSala, "La sala no debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarSala(idSala), "El idsala generado no debería existir en el diccionario de partidas.");


        }
         [TestMethod]
        public void CrearSala_ParametroConEspacios_RetornaFalse()
        {
            // Arrange
            string anfitrion = "         ";

            // Act
            string idSala = manejador.CrearSala(anfitrion);

            // Assert
            Assert.IsNull(idSala, "El idsala es null.");

        }

        # endregion CrearSala

        #region ValidarSala

        [TestMethod]
        public void ValidarPartida_PartidaCreada_ExisteEnDiccionario()
        {
            // Arrange
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Act
            bool resultado = manejador.ValidarPartida(idPartida);

            // Assert
            Assert.IsTrue(resultado, "Debería devolver true para una partida recién creada.");
        }
        [TestMethod]
        public async void ValidarPartida_PartidaTodosJugadoresAbandonan_NoExisteEnDiccionario()
        {
            // Arrange
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioAnfritrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 1, Nombre = "NaviKing" };

            // Arrange
            var idPartida = manejador.CrearPartida(usuarioAnfritrion.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioAnfritrion.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);

            implementacionCallback?.Close();

            // Act
            bool resultado = manejador.ValidarPartida(idPartida);

            // Assert
            Assert.IsFalse(resultado, "Debería devolver false porque se debio eliminar.");
        }
        [TestMethod]
        public void ValidarPartida_PartidaNoCreada_NoExisteEnDiccionario()
        {
            // Arrange
            string idPartidaInexistente = "salaNoCreada";

            // Act
            bool resultado = manejador.ValidarPartida(idPartidaInexistente);

            // Assert
            Assert.IsFalse(resultado, "Debería devolver false para una partida que no existe en el diccionario.");
        }
        [TestMethod]
        public void ValidarPartida_IdentificadorNull_DeberiaRetornarFalse()
        {
            // Act
            bool resultado = manejador.ValidarPartida(null);

            // Assert
            Assert.IsFalse(resultado, "Debería devolver false cuando el identificador de la sala es null.");
        }

        #endregion ValidarSala
    }
}
