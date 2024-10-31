using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Contratos;
namespace Pruebas.Servidor
{
    /// <summary>
    /// Prueba de <see cref="WcfServicioLibreria.Contratos.IServicioPartida"/>
    /// </summary>
    [TestClass]
    public class ServicioPartida
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;
        private ConfiguracionPartida configuracionGenerica;
        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
            configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 0);
        }
        #region CrearPartida
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosMixta_DeberiaRetornarIdPartida()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mixta,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosAnimales_DeberiaRetornarIdPartida()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Animales,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosPaises_DeberiaRetornarIdPartida()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Paises,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosMitologia_DeberiaRetornarIdPartida()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mitologia,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoAnfitrionEsNulo_DeberiaRetornarNull()
        {
            // Arrange
            string anfitrion = null;
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mixta,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNull(idPartida, "El idPartida debería ser null si el anfitrión es nulo.");
        }

        [TestMethod]
        public void CrearPartida_CuandoConfiguracionEsNula_DeberiaRetornarNull()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            ConfiguracionPartida configuracion = null;

            // Act
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            // Assert
            Assert.IsNull(idPartida, "El idPartida debería ser null si la configuración es nula.");
        }
        [TestMethod]
        public void CrearPartida_CuandoSeCreanMultiplesPartidas_DeberiaGenerarIdsUnicos()
        {
            // Arrange
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Animales,
                CondicionVictoriaPartida.PorCantidadRondas,
                6);
            int numeroDePartidas = 5;
            HashSet<string> idsPartidas = new HashSet<string>();

            // Act
            for (int i = 0; i < numeroDePartidas; i++)
            {
                string idPartida = manejador.CrearPartida(anfitrion, configuracion);
                Assert.IsNotNull(idPartida, $"El idPartida debería ser válido para la partida {i + 1}.");
                bool idUnico = idsPartidas.Add(idPartida);
                Assert.IsTrue(idUnico, $"El idPartida '{idPartida}' debería ser único.");
            }

            // Assert: Verificar que se crearon los ids únicos
            Assert.AreEqual(numeroDePartidas, idsPartidas.Count, "Cada partida debería tener un idPartida único.");
        }
        #endregion

        #region ValidarPartida
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
        public void ValidarPartida_PartidaTodosJugadoresAbandonan_NoExisteEnDiccionario()
        {
            // Arrange
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioAnfritrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 1, Nombre = "NaviKing" };

            // Arrange
            var idPartida = manejador.CrearPartida(usuarioAnfritrion.Nombre, configuracionGenerica);
            manejador.UnirsePartida(usuarioAnfritrion.Nombre, idPartida);
            manejador.UnirsePartida(usuarioNuevo.Nombre, idPartida);

            if (implementacionCallback !=null)
            {
                implementacionCallback.Close();
            }

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

        #endregion

        #region EsPartidaEmpezada
        [TestMethod]
        public void EsPartidaEmpezada_IdentificadorNull_DeberiaRetornarFalse()
        {
            // Act
            bool resultado = manejador.EsPartidaEmpezada(null);

            // Assert
            Assert.IsFalse(resultado, "Debería devolver false cuando el identificador de la sala es null.");

        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoEmpezada_RetornaTrue()
        {
            // Arrange
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            //TODO:UNISER Y empezar la partiada
            // Act
            bool resultado = manejador.EsPartidaEmpezada(idPartida);

            // Assert
            Assert.IsTrue(resultado, "Debería devolver true para una partida emepzada.");
        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoEmpezada_RetornaFalse()
        {
            // Arrange
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);
            //TODO:UNISER Y empezar la partiada
            
            // Act
            bool resultado = manejador.EsPartidaEmpezada(idPartida);

            // Assert
            Assert.IsTrue(resultado, "Debería devolver false para una partida emepzada.");
        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoCreada_NoExisteEnDiccionario()
        {
            // Arrange
            string idPartidaInexistente = "salaNoCreada";

            // Act
            bool resultado = manejador.EsPartidaEmpezada(idPartidaInexistente);

            // Assert
            Assert.IsFalse(resultado, "Debería devolver false para una partida que no existe en el diccionario.");
        }
        

        #endregion EsPartidaEmpezada

    }

}
