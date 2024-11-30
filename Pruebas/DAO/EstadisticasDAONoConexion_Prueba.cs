using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class EstadisticasDAONoConexion_Prueba : ConfiguracionPruebaBDInvalida
    {
        EstadisticasDAO estadisticasDAO = new EstadisticasDAO();
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_NoHayConexion_DeberiaMenosUno()
        {
            int id = 1;
            int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(id);
            Assert.AreEqual(resultado , -1, "Debe ser -1");

        }


        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_SinConexionBD_DeberiaRetornarMenosUno()
        {
            int idUsuario = 1;
                int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);

            
            Assert.AreEqual(-1, resultado, "El método debería devolver -1 cuando no hay conexión a la base de datos.");
        }
    }
}
