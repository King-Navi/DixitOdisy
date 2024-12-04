using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioSala_Prueba : ConfiguradorPruebaParaServicio
    {
        private ConfiguracionPartida configuracionGenerica;

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
    }
}
