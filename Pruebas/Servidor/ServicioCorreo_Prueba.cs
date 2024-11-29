using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioCorreo_Prueba
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;
        private const string CORREO_VALIDO = "zs22013698@estudiantes.uv.mx";
        private const string CORREO_INVALIDO = "zs220136estudiantes.uv.";
        private const string CODIGO_INVALIDO = "CodigoInvalido";

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }

        #region VerificarCorreo
        [TestMethod]
        public async void VerificarCorreo_CorreoValido_RetornaTrue()
        {
            //Arrage
            Usuario usuario = new Usuario
            {
                Correo = CORREO_VALIDO
            };
            //Act 
            bool result = await manejador.VerificarCorreoAsync(usuario);
            //Result
            Assert.IsTrue(result, "El código ha sido enviado al correo");
        }

        [TestMethod]
        public async void VerificarCorreo_CorreoInvalido_RetornaFalse()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Correo = CORREO_INVALIDO;
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            //Act 
            bool result = await manejador.VerificarCorreoAsync(usuario);
            //Result
            Assert.IsTrue(result, "El código ha sido enviado al correo");
        }

        [TestMethod]
        public async void VerificarCorreo_UsuarioNulo_RetornaFalse()
        {
            //Act 
            bool result = await manejador.VerificarCorreoAsync(null);
            //Result
            Assert.IsFalse(result, "El código no ha sido enviado");
        }

        #endregion VerificarCorreo

        #region VerificarCodigo
        [TestMethod]
        public void VerificarCodigo_CodigoNoCoincide_RetornaFalse()
        {
            // Act
            bool result = manejador.VerificarCodigo(CODIGO_INVALIDO, CORREO_VALIDO);
            // Assert
            Assert.IsFalse(result, "El código recibido no coincide con el generado.");
        }
        [TestMethod]
        public async void VerificarCodigo_CuandoCodigoNulo_DeberiaRetornarTrue()
        {
            // Arrange
            await manejador.VerificarCorreoAsync(new Usuario()
            {
                Correo = CORREO_VALIDO
            });

            // Act
            bool resultado = manejador.VerificarCodigo(null, CORREO_VALIDO);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver true cuando el código recibido coincide con el guardado.");
        }
        [TestMethod]
        public async void VerificarCodigo_CuandoNulo_DeberiaRetornarTrue()
        {
            // Arrange
            await manejador.VerificarCorreoAsync(new Usuario()
            {
                Correo = CORREO_VALIDO
            });

            // Act
            bool resultado = manejador.VerificarCodigo(null, null);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver true cuando el código recibido coincide con el guardado.");
        }
        
        #endregion

        #region EnviarCorreoAsync
        [TestMethod]
        public async void EnviarCorreoAsync_CuandoDatosSonValidos_DeberiaRetornarTrue()
        {
            // Arrange
            string codigoValido = "123456";

            // Act
            bool resultado = await manejador.EnviarCorreoAsync(codigoValido, CORREO_VALIDO);

            // Assert
            Assert.IsTrue(resultado, "El método debería devolver true cuando el correo se envía correctamente.");
        }
        [TestMethod]
        public async void EnviarCorreoAsync_CuandoCorreoEsInvalido_DeberiaRetornarTrue()
        {
            // Arrange
            string codigoValido = "123456";

            // Act
            bool resultado = await manejador.EnviarCorreoAsync(codigoValido, CORREO_VALIDO);

            // Assert
            Assert.IsTrue(resultado, "El método debería devolver true cuando el correo del destinatario es inválido.");
        }

        [TestMethod]
        public async void EnviarCorreoAsync_CuandoServidorNoDisponible_DeberiaRetornarFalse()
        {
            // Arrange
            //Precondicon smtp no disponible
            string codigo = "123456";

            // Act
            bool resultado = await manejador.EnviarCorreoAsync(codigo, CORREO_VALIDO);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando el servidor SMTP no está disponible.");
        }

        #endregion EnviarCorreoAsync
    }
}
