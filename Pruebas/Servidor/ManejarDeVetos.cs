using DAOLibreria;
using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo.Vetos;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ManejarDeVetos
    {
        private const string NOMBRE_PROHIBIDO = "guest";
        private const string NOMBRE_PROHIBIDO_MAYUSCULAS = "GUest";

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }

        }

        [TestMethod]
        public async Task VetaJugador_CuandoNombreProhibido_DeberiaRetornarFalse()
        {
            // Arrange
            var manejadorDeVetos = new ManejadorDeVetos();
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
            var manejadorDeVetos = new ManejadorDeVetos();
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
            var manejadorDeVetos = new ManejadorDeVetos();
            //Pre condcion: debe ser un usaurio valido
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
            var manejadorDeVetos = new ManejadorDeVetos();
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
            var manejadorDeVetos = new ManejadorDeVetos();
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
            var manejadorDeVetos = new ManejadorDeVetos();
            //Pre condcion: debe ser un usaurio valido
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
