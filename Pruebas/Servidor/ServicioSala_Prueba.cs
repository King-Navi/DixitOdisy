using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioSala_Prueba : ConfiguradorPruebaParaServicio
    {
        private ConfiguracionPartida configuracionGenerica =new ConfiguracionPartida(
            TematicaPartida.Mitologia,           
            CondicionVictoriaPartida.PorCartasAgotadas, 
            10                                    
        );
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<System.DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }


        #region CrearSala
        [TestMethod]
        public void CrearSala_ParametroNull_RetornaFalse()
        {
            string anfitrion = null;
            string idSala = manejador.CrearSala(anfitrion);
            Assert.IsNull(idSala, "El idsala es null.");
        }
        [TestMethod]
        public void CrearSala_ParametroValido_RetornaFalse()
        {
            string anfitrion = "Navi";
            string idSala = manejador.CrearSala(anfitrion);
            Assert.IsNotNull(idSala, "La sala no debería haber sido generado.");
            Assert.IsTrue(manejador.ValidarSala(idSala), "El idsala generado no debería existir en el diccionario de partidas.");


        }
         [TestMethod]
        public void CrearSala_ParametroConEspacios_RetornaFalse()
        {
            string anfitrion = "         ";
            string idSala = manejador.CrearSala(anfitrion);
            Assert.IsNull(idSala, "El idsala es null.");
        }

        #endregion CrearSala

        #region ValidarSala
        [TestMethod]
        public void ValidarSala_SalaExistente_DeberiaRetornarTrue()
        {
            string anfitrion = "Hester123";
            string idSala = manejador.CrearSala(anfitrion);
            var resultado = manejador.ValidarSala(idSala);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void ValidarSala_SalaInexistente_DeberiaRetornarFalse()
        {
            var idSala = "salaNoExiste";
            var resultado = manejador.ValidarSala(idSala);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void ValidarSala_Nulo_DeberiaRetornaFalse()
        {
            var resultado = manejador.ValidarSala(null);
            Assert.IsFalse(resultado);
        }


        #endregion

        #region ValidarSala

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
        public async void ValidarPartida_PartidaTodosJugadoresAbandonan_NoExisteEnDiccionario()
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

        #endregion ValidarSala
    }
}
