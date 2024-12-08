using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioCuentaDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        private UsuarioCuentaDAO usuarioCuentaDAO = new UsuarioCuentaDAO();
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_BaseDatosFalla_DeberiaRetornarMenosUno()
        {
            int idUsuario = 1;
            int idUsuarioCuentaEsperado = -1;

            var resultado = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(idUsuario);

            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado no coincide con el esperado.");
        }
    }
}
