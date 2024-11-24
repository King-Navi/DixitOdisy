using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioCuentaDAO_Prueba : ConfiguracionPruebaBD
    {
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioExistente_DeberiaRetornarIdUsuarioCuenta()
        {
            // Arrange
            // Precondición: El ID 1 debe existir en la base de datos
            int idUsuario = 1;
            // Precondición: El ID de la cuenta esperada para este usuario
            int idUsuarioCuentaEsperado = 1; 

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(idUsuario);

            // Assert
            Assert.IsNotNull(resultado, "El método debería retornar un valor no nulo.");
            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado no coincide con el esperado.");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistenteNegativo_DeberiaRetornarNull()
        {
            // Arrange
            // Precondición: Este ID no debería existir en la base de datos
            int idUsuarioInexistente = -130;

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(idUsuarioInexistente);

            // Assert
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        }
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_IdUsuarioInexistentePositvo_DeberiaRetornarNull()
        {
            // Arrange
            // Precondición: Este ID no debería existir en la base de datos
            int idUsuarioInexistente = -123734;

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(idUsuarioInexistente);

            // Assert
            Assert.IsNull(resultado, "El método debería retornar null para un ID de usuario inexistente.");
        }
    }
}
