using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class VetoDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int IDUSUARIOCUENTA_PRUEBA = 2;
        private VetoDAO vetoDAO = new VetoDAO();
        private const string NOMBRE = "DescribeloEntities";

        [TestCleanup]
        public void LimpiarRegistrosDePrueba()
        {
            using (var context = new DescribeloEntities())
            {
                var expulsiones = context.Veto.Where(e => e.idUsuarioCuenta == IDUSUARIOCUENTA_PRUEBA).ToList();
                context.Veto.RemoveRange(expulsiones);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public async Task VerificarConexionAsync_CuandoConexionEsExitosa_DeberiaRetornarTrue()
        {
            
            // Precondición: La base de datos debe estar disponible y configurada correctamente.
            
            bool resultado = await DAOLibreria.ModeloBD.Conexion.VerificarConexionAsync();

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando la conexión a la base de datos es exitosa.");
        }
        [TestMethod]
        public void ExisteVetoPorIdCuenta_IdCuenta_DeberiaRetornarFalse()
        {
            
            // Precondición: El ID 1 debe existir en la base de datos con tabala asociada
            DateTime fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;
            AgregarVeto(IDUSUARIOCUENTA_PRUEBA, DateTime.Now, esPermanente, fechaFin);
            
            bool resultado = vetoDAO.ExisteTablaVetoPorIdCuenta(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsTrue(resultado, "El método debería retornar true.");
        }
        [TestMethod]
        public void ExisteVetoPorIdCuenta_IdCuentaSinVetoTabla_DeberiaRetornarFalse()
        {
            
            // Precondición: El ID NO debe existir en la base de datos con un veto asociado
            int idUsuarioCuenta = -1321; 

            
            bool resultado = vetoDAO.ExisteTablaVetoPorIdCuenta(idUsuarioCuenta);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando el idUsuarioCuenta no tiene una tabla asociada.");
        }
        [TestMethod]
        public void CrearRegistroVeto_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            
            int idUsuarioCuenta = 2; 
            DateTime? fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;

            
            bool resultado = vetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando los datos son válidos y el veto se crea correctamente.");
        }
        [TestMethod]
        public void CrearRegistroVeto_CuandoIdUsuarioCuentaInvalido_DeberiaRetornarFalse()
        {
            
            int idUsuarioCuenta = -14; // ID de usuario inválido
            DateTime fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;
            
            bool resultado = vetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando el ID de usuario cuenta es inválido.");
        }

        [TestMethod]
        public void VerificarVetoPorIdCuenta_CuandoNoHayVetos_DeberiaRetornarFalse()
        {
            

            
            bool resultado = vetoDAO.VerificarVetoPorIdCuenta(IDUSUARIOCUENTA_PRUEBA);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando no hay vetos asociados al ID.");
        }
        [TestMethod]
        [ExpectedException(typeof(VetoPermanenteExcepcion))]
        public void VerificarVetoPorIdCuenta_CuandoHayVetoPermanente_DeberiaLanzarExcepcion()
        {
            
            // Un ID con veto permanente
            AgregarVetoPermanente(IDUSUARIOCUENTA_PRUEBA);
            
            vetoDAO.VerificarVetoPorIdCuenta(IDUSUARIOCUENTA_PRUEBA);

            : Verificado por ExpectedException
        }
        [TestMethod]
        [ExpectedException(typeof(VetoEnProgresoExcepcion))]
        public void VerificarVetoPorIdCuenta_CuandoHayVetoEnProgreso_DeberiaLanzarExcepcion()
        {
            
            DateTime fechaFin = DateTime.Now.AddDays(7);
            bool esPermanente = false;
            AgregarVeto(IDUSUARIOCUENTA_PRUEBA, DateTime.Now, esPermanente, fechaFin);

            
            vetoDAO.VerificarVetoPorIdCuenta(IDUSUARIOCUENTA_PRUEBA);

            : Verificado por ExpectedException
        }
        public void AgregarVetoPermanente(int idUsuarioCuenta)
        {
            using (var context = new DescribeloEntities())
            {
                var vetoPermanente = new Veto
                {
                    idUsuarioCuenta = idUsuarioCuenta,
                    fechaInicio=DateTime.Now,
                    esPermanente = true,
                    fechaFin = null
                };
                context.Veto.Add(vetoPermanente);
                context.SaveChanges();
            }
        }
        public void AgregarVeto(int idUsuarioCuenta, DateTime _fechaIncio, bool _esPermanente, DateTime _fechaFin)
        {
            using (var context = new DescribeloEntities())
            {
                var vetoPermanente = new Veto
                {
                    idUsuarioCuenta = idUsuarioCuenta,
                    fechaInicio= _fechaIncio,
                    esPermanente = _esPermanente,
                    fechaFin = _fechaFin
                };
                context.Veto.Add(vetoPermanente);
                context.SaveChanges();
            }
        }
    }
}
