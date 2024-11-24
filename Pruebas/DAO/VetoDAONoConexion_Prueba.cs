using DAOLibreria;
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
    public class VetoDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        [TestMethod]
        public async Task VerificarConexionAsync_CuandoConexionFalla_DeberiaRetornarFalse()
        {
            // Arrange
            // Precondicon: Configura una conexión incorrecta para simular la falla de conexión

            // Act
            bool resultado = await DAOLibreria.ModeloBD.Conexion.VerificarConexionAsync();

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando la conexión a la base de datos falla.");

            // Restaura la configuración original después de la prueba, si es necesario
            ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "usuarioValido", "contraseniaValida");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_BaseDatosFalla_DeberiaRetornarFalse()
        {
            // Arrange
            // Precondición: BD caida
            int idUsuario = 1;

            // Act
            var resultado = VetoDAO.ExisteTablaVetoPorIdCuenta(idUsuario);

            // Assert
            Assert.IsFalse(resultado, "Dberia retorna false.");
        }
        [TestMethod]
        public void CrearTablaVeto_CuandoBaseDatosFalla_DeberiaRetornarFalse()
        {
            // Arrange
            int idUsuarioCuenta = 1; // ID válido
            DateTime? fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;

            // Act
            bool resultado = VetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando ocurre una excepción en el contexto.");
        }
    }
}
