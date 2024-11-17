using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class EstadisticasDAONoConexion_Prueba
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
    }
}
