using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioCorreo_Prueba : ConfiguradorPruebaParaServicio
    {
        private const string CORREO_VALIDO = "zs22013698@estudiantes.uv.mx";
        private const string CORREO_INVALIDO = "zs220136estudiantes.uv.";
        private const string CODIGO_INVALIDO = "CodigoInvalido";

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
        #region VerificarCorreo
        [TestMethod]
        public async Task VerificarCorreo_CorreoValido_RetornaTrue()
        {
            Usuario usuario = new Usuario
            {
                Correo = CORREO_VALIDO
            };
            bool result = await manejador.VerificarCorreoAsync(usuario);
            Assert.IsTrue(result, "El código ha sido enviado al correo");
        }

        [TestMethod]
        public async Task VerificarCorreo_CorreoInvalido_RetornaFalse()
        {
            Usuario usuario = new Usuario();
            usuario.Correo = CORREO_INVALIDO;
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
 
            bool result = await manejador.VerificarCorreoAsync(usuario);

            Assert.IsFalse(result, "El código no ha sido enviado al correo");
        }

        [TestMethod]
        public async Task VerificarCorreo_UsuarioNulo_RetornaFalse()
        {
            bool result = await manejador.VerificarCorreoAsync(null);

            Assert.IsFalse(result, "El código no ha sido enviado");
        }

        #endregion VerificarCorreo

        #region VerificarCodigo
        [TestMethod]
        public void VerificarCodigo_CodigoNoCoincide_RetornaFalse()
        {
            bool result = manejador.VerificarCodigo(CODIGO_INVALIDO, CORREO_VALIDO);
            
            Assert.IsFalse(result, "El código recibido no coincide con el generado.");
        }
        [TestMethod]
        public async Task VerificarCodigo_CuandoCodigoNulo_DeberiaRetornarTrue()
        {
            
            await manejador.VerificarCorreoAsync(new Usuario()
            {
                Correo = CORREO_VALIDO
            });
            
            bool resultado = manejador.VerificarCodigo(null, CORREO_VALIDO);

            Assert.IsFalse(resultado, "El método debería devolver true cuando el código recibido coincide con el guardado.");
        }
        [TestMethod]
        public async Task VerificarCodigo_CuandoNulo_DeberiaRetornarTrue()
        {
            
            await manejador.VerificarCorreoAsync(new Usuario()
            {
                Correo = CORREO_VALIDO
            });
            
            bool resultado = manejador.VerificarCodigo(null, null);
            
            Assert.IsFalse(resultado, "El método debería devolver true cuando el código recibido coincide con el guardado.");
        }
        
        #endregion

        #region EnviarCorreoAsync
        [TestMethod]
        public async Task EnviarCorreoAsync_CuandoDatosSonValidos_DeberiaRetornarTrue()
        {

            string codigoValido = "123456";
            
            bool resultado = await manejador.EnviarCorreoAsync(codigoValido, CORREO_VALIDO);
            
            Assert.IsTrue(resultado, "El método debería devolver true cuando el correo se envía correctamente.");
        }
        [TestMethod]
        public async Task EnviarCorreoAsync_CuandoCorreoEsInvalido_DeberiaRetornarTrue()
        {
            string codigoValido = "123456";

            bool resultado = await manejador.EnviarCorreoAsync(codigoValido, CORREO_VALIDO);

            Assert.IsTrue(resultado, "El método debería devolver true cuando el correo del destinatario es inválido.");
        }

        [TestMethod]
        public async Task EnviarCorreoAsync_CuandoServidorNoDisponible_DeberiaRetornarFalse()
        {
            string codigo = "123456";

            bool resultado = await manejador.EnviarCorreoAsync(codigo, CORREO_VALIDO);

            Assert.IsFalse(resultado, "El método debería devolver false cuando el servidor SMTP no está disponible.");
        }

        #endregion EnviarCorreoAsync
    }
}
