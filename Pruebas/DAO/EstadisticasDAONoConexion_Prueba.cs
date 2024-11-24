using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class EstadisticasDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_NoHayConexion_DeberiaMenosUno()
        {
            //Arrage
            //Precondcion: BD caida
            int id = 1;

            //Act 
            int resultado = DAOLibreria.DAO.EstadisticasDAO.ObtenerIdEstadisticaConIdUsuario(id);
            //Assert
            Assert.AreEqual(resultado , -1, "Debe ser -1");

        }

        [TestMethod]
        public void ColocarUltimaConexion_NoHayConexion_RetornaFalse()
        {
            // Arrange
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioExistente = 1;
            // Act
            bool resultado = DAOLibreria.DAO.UsuarioDAO.ColocarUltimaConexion(idUsuarioExistente);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar fasle.");

        }

        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_SinConexionBD_DeberiaRetornarMenosUno()
        {
            // Arrange
            //Precondcion: El ID debe exisitir en BD.
            int idUsuario = 1; 

            // Act
            int resultado = EstadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);

            // Assert
            Assert.AreEqual(-1, resultado, "El método debería devolver -1 cuando no hay conexión a la base de datos.");
        }
    }
}
