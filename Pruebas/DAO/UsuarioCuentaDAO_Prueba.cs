using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioCuentaDAO_Prueba : ConfiguracionPruebaBD
    {
        private const string NOMBRE_VALIDO = "naviking";
        private const string CONTRASENIA_VALIDA = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B";
        private const string CONTRASENIA_INCORRECTA = "1234";
        private const int IDUSUARIO_INEXISTENTE = -130;
        private const int IDUSUARIO = 1;

        #region ObtenerIdUsuarioCuentaPorIdUsuario
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioExistente_DeberiaRetornarIdUsuarioCuenta()
        {
            // Arrange
            int idUsuarioCuentaEsperado = IDUSUARIO;

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO);

            // Assert
            Assert.IsNotNull(resultado, "El método debería retornar un valor no nulo.");
            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado no coincide con el esperado.");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistenteNegativo_DeberiaRetornarNull()
        {
            // Arrange

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);

            // Assert
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        }
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistentePositvo_DeberiaRetornarNull()
        {
            // Arrange

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(IDUSUARIO_INEXISTENTE);

            // Assert
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        } 
        #endregion

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CuandoCredencialesSonValidas_DeberiaRetornarUsuario()
        {
            // Arrange

            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(NOMBRE_VALIDO, CONTRASENIA_VALIDA);
            // Assert
            Assert.IsNotNull(usuario, "El usuario debería ser retornado cuando las credenciales son válidas.");
            Assert.AreEqual(NOMBRE_VALIDO, usuario.NombreUsuario, "El gamertag del usuario retornado debería coincidir con el gamertag proporcionado.");
        }

        [TestMethod]
        public void ValidarCredenciales_CuandoContraseniaEsIncorrecta_DeberiaRetornarNull()
        {
            // Arrange

            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(NOMBRE_VALIDO, CONTRASENIA_INCORRECTA);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando la contraseña es incorrecta.");
        }
        [TestMethod]
        public void ValidarCredenciales_CuandoEsNulo_DeberiaRetornarNull()
        {
            // Arrange
            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(null, null);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando el gamertag no existe en la base de datos.");
        }
        #endregion
    }
}
