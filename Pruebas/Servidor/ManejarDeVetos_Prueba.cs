using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo.Vetos;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ManejarDeVetos_Prueba : ConfiguradorPruebaParaServicio
    {
        private const string NOMBRE_PROHIBIDO = "guest";
        private const string NOMBRE_PROHIBIDO_MAYUSCULAS = "GUest";

        [TestInitialize]
        protected override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);
        }
        [TestCleanup]
        protected override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }



        [TestMethod]
        public async Task VetaJugador_CuandoNombreProhibido_DeberiaRetornarFalse()
        {
            // Arrange

            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object,imitarUsuarioCuentaDAO.Object ,imitarExpulsionDAO.Object);
            string nombreJugador = "Player" + NOMBRE_PROHIBIDO;
            string nombreJugadorMayusculasProhido =  NOMBRE_PROHIBIDO_MAYUSCULAS + "Player" ;

            // Act
            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);
            bool resultadoMayusculas = await manejadorDeVetos.VetaJugadorAsync(nombreJugadorMayusculasProhido);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas.");
            Assert.IsFalse(resultadoMayusculas, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas.");
        }

        [TestMethod]
        public async Task VetaJugador_CuandoJugadorNoExiste_DeberiaRetornarFalse()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "jugadorNoExiste";

            // Act
            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el jugador no existe en la base de datos.");
        }
        [TestMethod]
        public async Task VetaJugador_CuandoJugadorEsValido_DeberiaRetornarTrue()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "user2";
            // Act
            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando el jugador es válido y se crea un registro de veto.");
        }

        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoNombreProhibido_DeberiaRetornarFalse()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "guestPlayer";
            string motivo = "Conducta inapropiada";
            bool esHacker = false;

            // Act
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas.");
        }
        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoJugadorNoExiste_DeberiaRetornarFalse()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            //Pre condcion: NO debe ser un usaurio valido
            string nombreJugador = "JugadorInexistente";
            string motivo = "Hackeo detectado";
            bool esHacker = true;

            // Act
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el jugador no existe en la base de datos.");
        }

        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);

            string nombreJugador = "user2";
            string motivo = "Uso de software no permitido";
            bool esHacker = true;

            // Act
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando los datos son válidos y se registra una expulsión correctamente.");
        }

        
    }
}
