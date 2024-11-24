using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.DAO.Utilidades;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioIniciarSesionPruebas : ConfiguracionPruebaBD
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }

        [TestMethod]
        public void ValidarCredenciales_CredencialesCorrectas_DeberiaRetornarUsuario()
        {
            // Arrange
            //Pre condicion, el usuario debe exisitir en BD
            string gamertagValido = "elrevo";
            string contraseniaValida = "83DB3ABE8D166668D1B09657EB82F4E17361DE34BD389BA7B0566811DAA68703";
            var usuarioEsperado = new DAOLibreria.ModeloBD.UsuarioPerfilDTO
            {
                NombreUsuario = gamertagValido,
            };

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagValido, contraseniaValida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(usuarioEsperado.NombreUsuario, resultado.Nombre, "El nombre del usuario debería coincidir.");
        }
        [TestMethod]
        public void ValidarCredenciales_CredencialesIncorrectas_DeberiaRetornarNulo()
        {
            // Arrange
            string gamertagInvalido = "UsuarioInvalidoParaPruebas";
            string contraseniaInvalida = "ContraseniaIncorrecta123";

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
        [TestMethod]
        public void ValidarCredenciales_ValorNulo_DeberiaRetornarNulo()
        {
            // Arrange
            string gamertagInvalido = null;
            string contraseniaInvalida = null;

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
    }
}
