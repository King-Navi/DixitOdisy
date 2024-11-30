using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class ExpulsionDAO_Prueba : ConfiguracionPruebaBD
    {
        //precondicion: ID fijo para todas las pruebas
        private const int IDUSUARIOCUENTA_PRUEBA = 2;
        private const string MOTIVO_PRUEBA = "PRUEBA MOTIVO";
        private const bool ES_HACKER_FALSE = false;
        private const bool ES_HACKER_TRUE = true;
        private ExpulsionDAO  expulsionDAO= new ExpulsionDAO();
        [TestCleanup]
        public void LimpiarRegistrosDePrueba()
        {
            using (var context = new DescribeloEntities())
            {
                var expulsiones = context.Expulsion.Where(e => e.idUsuarioCuenta == IDUSUARIOCUENTA_PRUEBA).ToList();
                context.Expulsion.RemoveRange(expulsiones);
                context.SaveChanges();
            }
        }
        #region CrearRegistroExpulsion
        [TestMethod]
        public void CrearRegistroExpulsion_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            
            
            bool resultado = expulsionDAO.CrearRegistroExpulsion(IDUSUARIOCUENTA_PRUEBA, MOTIVO_PRUEBA, ES_HACKER_FALSE);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando los datos son válidos y se crea correctamente un registro de expulsión.");
        }
        [TestMethod]
        public void CrearRegistroExpulsion_CuandoIdUsuarioInvalido_DeberiaRetornarFalse()
        {
            
            // ID de usuario inválido
            int idUsuarioCuenta = -1;

            
            bool resultado = expulsionDAO.CrearRegistroExpulsion(idUsuarioCuenta, MOTIVO_PRUEBA, ES_HACKER_FALSE);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando el ID del usuario es inválido.");
        } 
        #endregion

        #region TieneMasDeDiezExpulsionesSinPenalizar
        [TestMethod]
        public void TieneMasDeDiezExpulsionesSinPenalizar_CuandoTieneMasDeDiez_DeberiaRetornarTrue()
        {
            
            PrepararExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA, 12);

            
            bool resultado = expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsTrue(resultado, "El método debería retornar true cuando el usuario tiene 10 o más expulsiones sin penalizar.");
        }
        [TestMethod]
        public void TieneMasDeDiezExpulsionesSinPenalizar_CuandoTieneMenosDeDiez_DeberiaRetornarFalse()
        {
            
            PrepararExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA, 5);

            
            bool resultado = expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el usuario tiene menos de 10 expulsiones sin penalizar.");
        }
        [TestMethod]
        public void TieneMasDeDiezExpulsionesSinPenalizar_CuandoNoTieneRegistros_DeberiaRetornarFalse()
        {
            
            
            bool resultado = expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el usuario no tiene registros de expulsión.");
        } 
        #endregion

        private void PrepararExpulsionesSinPenalizar(int idUsuarioCuenta, int cantidad)
        {
            using (var context = new DescribeloEntities())
            {
                for (int i = 0; i < cantidad; i++)
                {
                    Expulsion nuevaExpulsion = new Expulsion
                    {
                        idUsuarioCuenta = idUsuarioCuenta,
                        motivo = "Motivo de prueba",
                        fuePenalizado = false,
                        esHacker = false,
                    };
                    context.Expulsion.Add(nuevaExpulsion);
                }
                context.SaveChanges();
            }
        }

       
        #region CambiarExpulsionesAFueronPenalizadas
        [TestMethod]
        public void CambiarExpulsionesAFueronPenalizadas_CuandoHayExpulsionesSinPenalizar_DeberiaActualizarEstado()
        {
            
            PrepararExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA, 3);

            
            bool resultado = expulsionDAO.CambiarExpulsionesAFueronPenalizadas(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando se actualizan expulsiones sin penalizar.");

            using (var context = new DescribeloEntities())
            {
                var expulsiones = context.Expulsion
                                          .Where(e => e.idUsuarioCuenta == IDUSUARIOCUENTA_PRUEBA)
                                          .ToList();

                Assert.IsTrue(expulsiones.All(e => e.fuePenalizado), "Todas las expulsiones deberían estar penalizadas.");
            }
        }
        [TestMethod]
        public void CambiarExpulsionesAFueronPenalizadas_CuandoNoHayExpulsionesSinPenalizar_DeberiaRetornarTrue()
        {
            
            PrepararExpulsionesSinPenalizar(IDUSUARIOCUENTA_PRUEBA, 0);

            
            bool resultado = expulsionDAO.CambiarExpulsionesAFueronPenalizadas(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando no hay expulsiones sin penalizar.");
        }
        [TestMethod]
        public void CambiarExpulsionesAFueronPenalizadas_CuandoIdUsuarioEsInvalido_DeberiaRetornarFalse()
        {
            
            //Precondicion: es id invalido
            int idUsuarioCuentaInvalido = -1;

            
            bool resultado = expulsionDAO.CambiarExpulsionesAFueronPenalizadas(idUsuarioCuentaInvalido);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando el ID del usuario no es válido.");
        }
        #endregion
    }
}
