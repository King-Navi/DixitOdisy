﻿using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
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
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario());
            imitarUsuarioCuentaDAO.Setup(dao => dao.ObtenerIdUsuarioCuentaPorIdUsuario(It.IsAny<int>())).Returns(1);
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(true);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }



        [TestMethod]
        public async Task VetaJugador_CuandoNombreReservado_DeberiaRetornarFalse()
        {
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "Player" + NOMBRE_PROHIBIDO;

            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);

            Assert.IsFalse(resultado, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas.");
        }

        [TestMethod]
        public async Task VetaJugador_CuandoNombreConMayusculasReservado_DeberiaRetornarFalse()
        {
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugadorMayusculasProhido = NOMBRE_PROHIBIDO_MAYUSCULAS + "Player";

            bool resultadoMayusculas = await manejadorDeVetos.VetaJugadorAsync(nombreJugadorMayusculasProhido);

            Assert.IsFalse(resultadoMayusculas, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas en mayúsculas.");
        }


        [TestMethod]
        public async Task VetaJugador_CuandoJugadorNoExiste_DeberiaRetornarFalse()
        {
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns((Usuario)null);
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "jugadorNoExiste";
            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);
            Assert.IsFalse(resultado, "El método debería retornar false cuando el jugador no existe en la base de datos.");
        }
        [TestMethod]
        public async Task VetaJugador_CuandoJugadorEsValido_DeberiaRetornarTrue()
        {
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "user2";
            bool resultado = await manejadorDeVetos.VetaJugadorAsync(nombreJugador);
            Assert.IsTrue(resultado, "El método debería retornar true cuando el jugador es válido y se crea un registro de veto.");
        }

        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoNombreProhibido_DeberiaRetornarFalse()
        {
            
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            string nombreJugador = "guestPlayer";
            string motivo = "Conducta inapropiada";
            bool esHacker = false;

            
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el nombre del jugador contiene palabras prohibidas.");
        }
        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoJugadorNoExiste_DeberiaRetornarFalse()
        {
            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            manejadorDeVetos.Conexion = imitacionConexion.Object;
            string nombreJugador = "JugadorInexistente";
            string motivo = "Hackeo detectado";
            bool esHacker = true;
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);
            Assert.IsFalse(resultado, "El método debería retornar false cuando el jugador no existe en la base de datos.");
        }

        [TestMethod]
        public async Task RegistrarExpulsionJugador_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            imitarExpulsionDAO.Setup(dao => dao.TieneMasDeDiezExpulsionesSinPenalizar(It.IsAny<int>()))
                .Returns(false);
            imitarExpulsionDAO.Setup(dao => dao.CambiarExpulsionesAFueronPenalizadas(It.IsAny<int>()))
                .Returns((int idUsuarioCuenta) => idUsuarioCuenta > 0);
            imitarExpulsionDAO.Setup(dao => dao.CrearRegistroExpulsion(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(true);
            imitarExpulsionDAO.Setup(dao => dao.CrearRegistroExpulsion(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(true);


            var manejadorDeVetos = new ManejadorDeVetos(imitarVetoDAO.Object, imitarUsuarioDAO.Object, imitarUsuarioCuentaDAO.Object, imitarExpulsionDAO.Object);
            manejadorDeVetos.Conexion = imitacionConexion.Object;
            string nombreJugador = "user2";
            string motivo = "Uso de software no permitido";
            bool esHacker = true;
            bool resultado = await manejadorDeVetos.RegistrarExpulsionJugadorAsync(nombreJugador, motivo, esHacker);
            Assert.IsTrue(resultado, "El método debería retornar true cuando los datos son válidos y se registra una expulsión correctamente.");
        }

        
    }
}
