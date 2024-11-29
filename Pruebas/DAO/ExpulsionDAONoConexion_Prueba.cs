using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class ExpulsionDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        private ExpulsionDAO expulsionDAO = new ExpulsionDAO();

        [TestMethod]
        public void CrearRegistroExpulsion_CuandoContextoFalla_DeberiaRetornarFalse()
        {
            // Arrange
            // ID válido
            int idUsuarioCuenta = 1; 
            string motivo = "Uso de software no autorizado";
            bool esHacker = true;


            // Act
            bool resultado = expulsionDAO.CrearRegistroExpulsion(idUsuarioCuenta, motivo, esHacker);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando ocurre un fallo en el contexto.");

        }

        [TestMethod]
        public void TieneMasDeDiezExpulsionesSinPenalizar_CuandoContextoFalla_DeberiaRetornarFalse()
        {
            // Arrange
            // ID válido de usuario que no tiene registros de expulsión en la base de datos
            int idUsuarioCuenta = 3;

            // Act
            bool resultado = expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar(idUsuarioCuenta);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el usuario no tiene registros de expulsión.");
        } 
        [TestMethod]
        public void CambiarExpulsionesAFueronPenalizadas_CuandoContextoFalla_DeberiaRetornarFalse()
        {
            // Arrange
            // ID válido de usuario que no tiene registros de expulsión en la base de datos
            int idUsuarioCuenta = 1;

            // Act
            bool resultado = expulsionDAO.CambiarExpulsionesAFueronPenalizadas(idUsuarioCuenta);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el usuario no tiene registros de expulsión.");
        }
    }
}
