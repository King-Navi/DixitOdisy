using DAOLibreria;
using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class VetoDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        private VetoDAO vetoDAO= new VetoDAO();
        [TestMethod]
        public async Task VerificarConexionAsync_CuandoConexionFalla_DeberiaRetornarFalse()
        {
            
            // Precondicon: Configura una conexión incorrecta para simular la falla de conexión

            
            bool resultado = await DAOLibreria.ModeloBD.Conexion.VerificarConexionAsync();

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando la conexión a la base de datos falla.");

            // Restaura la configuración original después de la prueba, si es necesario
            ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "usuarioValido", "contraseniaValida");
        }

        [TestMethod]
        public void ObtenerIdUsuarioCuentaPorIdUsuario_BaseDatosFalla_DeberiaRetornarFalse()
        {
            
            // Precondición: BD caida
            int idUsuario = 1;

            
            var resultado = vetoDAO.ExisteTablaVetoPorIdCuenta(idUsuario);

            
            Assert.IsFalse(resultado, "Dberia retorna false.");
        }
        [TestMethod]
        public void CrearTablaVeto_CuandoBaseDatosFalla_DeberiaRetornarFalse()
        {
            
            int idUsuarioCuenta = 1; // ID válido
            DateTime? fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;

            
            bool resultado = vetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando ocurre una excepción en el contexto.");
        }
    }
}
