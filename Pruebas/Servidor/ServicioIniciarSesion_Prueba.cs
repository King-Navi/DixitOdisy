using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioIniciarSesion_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
        }
        [TestCleanup]

        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CredencialesCorrectas_DeberiaRetornarUsuario()
        {
            var nombreValido = "UsuarioPredeterminado";
            var contraseniaValida = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B";
            
            var resultado = manejador.ValidarCredenciales(nombreValido, contraseniaValida);

            Assert.IsNull(resultado, "El método debería devolver un usuario válido.");
        }
        [TestMethod]
        public void ValidarCredenciales_CredencialesIncorrectas_DeberiaRetornarNulo()
        {
            
            string gamertagInvalido = "UsuarioInvalidoParaPruebas";
            string contraseniaInvalida = "ContraseniaIncorrecta123";

            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
        [TestMethod]
        public void ValidarCredenciales_ValorNulo_DeberiaRetornarNulo()
        {
            
            string gamertagInvalido = null;
            string contraseniaInvalida = null;

            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }

        # endregion ValidarCredenciales
    }
}
