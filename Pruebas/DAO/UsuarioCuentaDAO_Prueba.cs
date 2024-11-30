using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioCuentaDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int IDUSUARIO_INEXISTENTE = -130;
        private const int IDUSUARIO = 1;
        private UsuarioCuentaDAO usuarioCuentaDAO = new UsuarioCuentaDAO();
        #region ObtenerIdUsuarioCuentaPorIdUsuario
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioExistente_DeberiaRetornarIdUsuarioCuenta()
        {
            
            int idUsuarioCuentaEsperado = IDUSUARIO;

            
            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO);

            
            Assert.IsNotNull(resultado, "El método debería retornar un valor no nulo.");
            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado no coincide con el esperado.");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistenteNegativo_DeberiaRetornarNull()
        {
            

            
            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);

            
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        }
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistentePositvo_DeberiaRetornarNull()
        {
            

            
            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);

            
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        } 
        #endregion


        #region VerificarCorreoConGamertag

        [TestMethod]
        public void VerificarCorreoConGamertag_CuandoCoinciden_DeberiaRetornarTrue()
        {
            
            var correo = "unaayjose@gmail.com";
            var gamertag = "unaay";

            
            bool resultado = usuarioCuentaDAO.ExisteUnicoUsuarioConGamertagYCorreo(gamertag, correo);

            
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
            
            string gamertag = "leoleo";
            string nuevoHashContrasenia = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd6d0fcb4c8b9e3fbb5"; // SHA256 de "password"

            
            bool resultado = usuarioCuentaDAO.EditarContraseniaPorGamertag(gamertag, nuevoHashContrasenia);

            
            Assert.IsTrue(resultado, "El método debería retornar true cuando los datos son válidos y la contraseña está en SHA256.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoContraseniaNoEsSHA256_DeberiaRetornarFalse()
        {
            
            string gamertag = "usuarioPrueba";
            string nuevoHashContraseniaInvalido = "password"; // Contraseña en texto plano, no en SHA256

            
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
