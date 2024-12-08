using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Contratos;
using Pruebas.Servidor.Utilidades;
using System;
using System.Threading.Tasks;
namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioPartida_Prueba : ConfiguradorPruebaParaServicio
    {
        private ConfiguracionPartida configuracionGenerica;
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

        #region CrearPartida
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosMixta_DeberiaRetornarIdPartida()
        {
            
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mixta,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosAnimales_DeberiaRetornarIdPartida()
        {
            
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Animales,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosPaises_DeberiaRetornarIdPartida()
        {
            
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Paises,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoDatosValidosMitologia_DeberiaRetornarIdPartida()
        {
            
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mitologia,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNotNull(idPartida, "El idPartida debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "El idPartida generado debería existir en el diccionario de partidas.");
        }
        [TestMethod]
        public void CrearPartida_CuandoAnfitrionEsNulo_DeberiaRetornarNull()
        {
            
            string anfitrion = null;
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Mixta,
                CondicionVictoriaPartida.PorCantidadRondas,
                12);

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNull(idPartida, "El idPartida debería ser null si el anfitrión es nulo.");
        }

        [TestMethod]
        public void CrearPartida_CuandoConfiguracionEsNula_DeberiaRetornarNull()
        {
            
            string anfitrion = "Anfitrion1";
            ConfiguracionPartida configuracion = null;

            
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            Assert.IsNull(idPartida, "El idPartida debería ser null si la configuración es nula.");
        }
        [TestMethod]
        public void CrearPartida_CuandoSeCreanMultiplesPartidas_DeberiaGenerarIdsUnicos()
        {
            
            string anfitrion = "Anfitrion1";
            var configuracion = new ConfiguracionPartida(
                TematicaPartida.Animales,
                CondicionVictoriaPartida.PorCantidadRondas,
                6);
            int numeroDePartidas = 5;
            HashSet<string> idsPartidas = new HashSet<string>();

            
            for (int i = 0; i < numeroDePartidas; i++)
            {
                string idPartida = manejador.CrearPartida(anfitrion, configuracion);
                Assert.IsNotNull(idPartida, $"El idPartida debería ser válido para la partida {i + 1}.");
                bool idUnico = idsPartidas.Add(idPartida);
                Assert.IsTrue(idUnico, $"El idPartida '{idPartida}' debería ser único.");
            }

            Assert.AreEqual(numeroDePartidas, idsPartidas.Count, "Cada partida debería tener un idPartida único.");
        }
        #endregion

        #region ValidarPartida
        [TestMethod]
        public void ValidarPartida_PartidaCreada_ExisteEnDiccionario()
        {
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);
            bool resultado = manejador.ValidarPartida(idPartida);
            Assert.IsTrue(resultado, "Debería devolver true para una partida recién creada.");
        }
        [TestMethod]
        public async Task ValidarPartida_PartidaTodosJugadoresAbandonan_NoExisteEnDiccionario()
        {
            var implementacionCallback = new PartidaCallbackImplementacion();
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
            var usuarioAnfritrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 1, Nombre = "NaviKing" };
            var idPartida = manejador.CrearPartida(usuarioAnfritrion.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioAnfritrion.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);
            implementacionCallback?.Close();
            bool resultado = manejador.ValidarPartida(idPartida);
            Assert.IsFalse(resultado, "Debería devolver false porque se debio eliminar.");
        }
       
        [TestMethod]
        public void ValidarPartida_PartidaNoCreada_NoExisteEnDiccionario()
        {
            string idPartidaInexistente = "salaNoCreada";
            bool resultado = manejador.ValidarPartida(idPartidaInexistente);
            Assert.IsFalse(resultado, "Debería devolver false para una partida que no existe en el diccionario.");
        }

        [TestMethod]
        public void ValidarPartida_IdentificadorNull_DeberiaRetornarFalse()
        {
            bool resultado = manejador.ValidarPartida(null);
            Assert.IsFalse(resultado, "Debería devolver false cuando el identificador de la sala es null.");
        }



        #endregion
        
        #region EsPartidaEmpezada
        [TestMethod]
        public void EsPartidaEmpezada_IdentificadorNull_DeberiaRetornarFalse()
        {
            
            bool resultado = manejador.EsPartidaEmpezada(null);

            
            Assert.IsFalse(resultado, "Debería devolver false cuando el identificador de la sala es null.");

        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoEmpezada_RetornaTrue()
        {
            
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);

            
            bool resultado = manejador.EsPartidaEmpezada(idPartida);

            
            Assert.IsTrue(resultado, "Debería devolver true para una partida emepzada.");
        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoEmpezada_RetornaFalse()
        {
            
            string anfitrion = "usuario123";
            var configuracion = configuracionGenerica;
            string idPartida = manejador.CrearPartida(anfitrion, configuracion);
            
            
            bool resultado = manejador.EsPartidaEmpezada(idPartida);

            
            Assert.IsTrue(resultado, "Debería devolver false para una partida emepzada.");
        }
        [TestMethod]
        public void EsPartidaEmpezada_PartidaNoCreada_NoExisteEnDiccionario()
        {
            
            string idPartidaInexistente = "salaNoCreada";

            
            bool resultado = manejador.EsPartidaEmpezada(idPartidaInexistente);

            
            Assert.IsFalse(resultado, "Debería devolver false para una partida que no existe en el diccionario.");
        }
        

        #endregion EsPartidaEmpezada

    }

}
