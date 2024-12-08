using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioCuentaDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int IDUSUARIO_INEXISTENTE = -130;
        private const int IDUSUARIO = 1;
        private const int RETORNO_USUARIO_NO_ENCONTRADO = -2;
        private UsuarioCuentaDAO usuarioCuentaDAO = new UsuarioCuentaDAO();

        [TestInitialize]
        public void BuscarUsuarioInicial()
        {
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario
                    .SingleOrDefault(u => u.idUsuario == IDUSUARIO);

                var usuarioCuenta = context.UsuarioCuenta
                    .SingleOrDefault(u => u.idUsuarioCuenta == IDUSUARIO);

                if (usuario != null && usuarioCuenta != null)
                {
                    usuarioInicial = usuario;
                    usuarioCuentaInicial = usuarioCuenta;
                }
                else
                {
                    Assert.Fail("No se encontro el usuario inicial");
                }
            }
        }
        #region ObtenerIdUsuarioCuentaPorIdUsuario
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioExistente_DeberiaRetornarIdUsuarioCuenta()
        {
            int idUsuarioCuentaEsperado = IDUSUARIO;
            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO);
            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado coincide con el esperado.");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistenteNegativo_DeberiaRetornarNull()
        {
            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);
             Assert.AreEqual(resultado, RETORNO_USUARIO_NO_ENCONTRADO);
        }
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistentePositvo_DeberiaRetornarNull()
        {
           var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);
            Assert.AreEqual(resultado, RETORNO_USUARIO_NO_ENCONTRADO);
        } 
        #endregion


        #region VerificarCorreoConGamertag

        [TestMethod]
        public void VerificarCorreoConGamertag_CuandoCoinciden_DeberiaRetornarTrue()
        {
            bool resultado = usuarioCuentaDAO.ExisteUnicoUsuarioConGamertagYCorreo(usuarioInicial.gamertag, usuarioCuentaInicial.correo);
            Assert.IsTrue(resultado, "El método debería retornar true cuando el correo y el gamertag coinciden.");
        }

        [TestMethod]
        public void VerificarCorreoConGamertag_CuandoNoCoinciden_DeberiaRetornarFalse()
        {
            
            var correo = "unaayjose@gmail.com";
            var gamertag = "unaay20";

            
            bool resultado = usuarioCuentaDAO.ExisteUnicoUsuarioConGamertagYCorreo(gamertag, correo);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el correo y el gamertag no coinciden.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            bool resultado = usuarioCuentaDAO.EditarContraseniaPorGamertag(POR_DEFECTO_NOMBRE_CUENTA, NUEVA_CONTRASENIA);
            Assert.IsTrue(resultado, "El método debería retornar true cuando los datos son válidos y la contraseña está en SHA256.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoContraseniaNoEsSHA256_DeberiaRetornarFalse()
        {
            
            string gamertag = "usuarioPrueba";
            string nuevoHashContraseniaInvalido = "password"; 

            
            bool resultado = usuarioCuentaDAO.EditarContraseniaPorGamertag(gamertag, nuevoHashContraseniaInvalido);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando la nueva contraseña no está en SHA256.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoGamertagYContraseniaSonNulos_DeberiaRetornarFalse()
        {
            
            string gamertagNulo = null;
            string nuevoHashContraseniaNulo = null;

            
            bool resultado = usuarioCuentaDAO.EditarContraseniaPorGamertag(gamertagNulo, nuevoHashContraseniaNulo);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el gamertag y la nueva contraseña son nulos.");
        }
        #endregion VerificarCorreoConGamertag

    }
}
