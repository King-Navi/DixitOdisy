using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.DAO.Utilidades;
using Pruebas.Servidor.Utilidades;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioIniciarSesion_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        protected override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
        }
        [TestCleanup]

        protected override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CredencialesCorrectas_DeberiaRetornarUsuario()
        {
            // Arrange
            imitarUsuarioDAO
            .Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>()))
            .Returns((string gamertag) =>
                {
                    // Simula el comportamiento del método
                    if (gamertag == "UsuarioExistente")
                    {
                        return new Usuario
                        {
                            idUsuario = 1,
                            gamertag = "UsuarioExistente",
                        };
                    }
                    return null; // Devuelve null si no encuentra el usuario
                });
            var nombreValido = "UsuarioExistente";
            var contraseniaValida = "contraseniaValida";
            // Act
            var resultado = manejador.ValidarCredenciales(nombreValido, contraseniaValida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(nombreValido, resultado.Nombre, "El nombre del usuario debería coincidir.");
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

        
        #endregion
    }
}
