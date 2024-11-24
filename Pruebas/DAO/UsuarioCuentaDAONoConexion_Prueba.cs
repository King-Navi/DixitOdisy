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
    public class UsuarioCuentaDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_BaseDatosFalla_DeberiaRetornarMenosUno()
        {
            // Arrange
            // Precondición: BD caida
            // Precondición: El ID 1 debe existir en la base de datos
            int idUsuario = 1;
            // Precondición: El ID de la cuenta esperada para este usuario
            int idUsuarioCuentaEsperado = -1;

            // Act
            var resultado = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(idUsuario);

            // Assert
            Assert.IsNotNull(resultado, "El método debería retornar un valor no nulo.");
            Assert.AreEqual(idUsuarioCuentaEsperado, resultado, "El ID de la cuenta retornado no coincide con el esperado.");
        }
    }
}
