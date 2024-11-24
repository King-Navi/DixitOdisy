using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioIniciarSesionPruebas
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
            string gamertagValido = "unaay";
            string contraseniaValida = "4327FE7955FF63FCF809A48C130B3546EFF6FEF893A396B4FEE083E853C0BA5C";
            var usuarioEsperado = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagValido,
                idUsuario = 2
            };


            // Act
            var resultado = manejador.ValidarCredenciales(gamertagValido, contraseniaValida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(usuarioEsperado.gamertag, resultado.Nombre, "El nombre del usuario debería coincidir.");
            Assert.AreEqual(usuarioEsperado.idUsuario, resultado.IdUsuario, "El ID del usuario debería coincidir.");
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
