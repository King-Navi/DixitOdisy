using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pruebas.DAO
{

    [TestClass]
    public class EstadisticasDAO_Prueba : ConfiguracionPruebaBD
    {
        private EstadisticasDAO estadisticasDAO = new EstadisticasDAO();
        #region RecuperarEstadisticas
        [TestMethod]
        public void RecuperarEstadisticas_IdExistente_DeberiaRetornarEstadistica()
        {
            
            //Precondicon: debe existir el id en base de datos
            int idEstadisticas = 1;

            
            var resultado = estadisticasDAO.RecuperarEstadisticas(idEstadisticas);

            
            Assert.IsNotNull(resultado, "El resultado no debería ser nulo.");
            Assert.AreEqual(idEstadisticas, resultado.idEstadisticas, "El ID de la estadística no coincide.");
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoValido_DeberiaRetornarExcepcion()
        {
            
            int idEstadisticas = -1;

             & Assert
            Assert.ThrowsException<ArgumentException>(() => estadisticasDAO.RecuperarEstadisticas(idEstadisticas), "Deber retornar un argument exceotion");
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoExiste_DeberiaRetornarExcepcion()
        {
            
            //Precondicon: NO debe existir el id en base de datos
            int idEstadisticas = 2030;

             & Assert

            Assert.ThrowsException<ArgumentException>(() => estadisticasDAO.RecuperarEstadisticas(idEstadisticas), "Deber retornar un argument exceotion");
        }
        #endregion RecuperarEstadisticas

        #region AgregarEstadisticaPartida

        [TestMethod]
        public async Task AgregarEstadisticaPartida_IdExistente_DeberiaActualizarEstadisticas()
        {
            
            //Precondcion: debe existir el id en base de datos
            int idEstadisticas = 1;
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;
            int victoria = 1;

            
            var resultado = await estadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria);

            
            Assert.IsTrue(resultado, "El resultado debería ser verdadero.");
        }
        [TestMethod]
        public async Task AgregarEstadisticaPartida_MultiplesAcciones_DeberiaActualizarEstadisticas()
        {
            
            //Precondicon: debe existir el id en base de datos
            int idEstadisticas = 1;
            int victoria = 1;
            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(idEstadisticas);
            // Lista de acciones a probar
            var acciones = new List<EstadisticasAcciones>
            {
                EstadisticasAcciones.IncrementarPartidaMixta,
                EstadisticasAcciones.IncrementarPartidaEspacio,
                EstadisticasAcciones.IncrementarPartidaPaises,
                EstadisticasAcciones.IncrementarPartidaAnimales,
                EstadisticasAcciones.IncrementarPartidaEspacio,
                EstadisticasAcciones.IncrementarPartidasMitologia
            };

            
            var tareasSolicitudes = new List<Task>();
            foreach (var accion in acciones)
            {
                tareasSolicitudes.Add(estadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria));
            }
            await Task.WhenAll(tareasSolicitudes);

            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(idEstadisticas);

            
            Assert.IsTrue(estadisticasAnteriores.partidasGanadas + acciones.Count == estadisticasNuevas.partidasGanadas, "El número de partidas ganadas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.partidasJugadas + acciones.Count == estadisticasNuevas.partidasJugadas, "El número de partidas jugadas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.vecesTematicaMixto + 1 == estadisticasNuevas.vecesTematicaMixto, "El número de partidas mixtas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.vecesTematicaAnimales + 1 == estadisticasNuevas.vecesTematicaAnimales, "El número de partidas mixtas no coincide.");

        }

        [TestMethod]
        public async Task AgregarEstadisticaPartida_VictoriaMayor_DeberiaRetornarExcepcion()
        {
            
            //Precondcion: debe existir el id en base de datos
            int idEstadisticas = 1;
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;
            int victoria = 2;


             & Assert
            await Assert.ThrowsExceptionAsync<ActividadSospechosaExcepcion>(async () => await estadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria), "Deber retornar un actividad sospechosa exceotion");
        }
        #endregion AgregarEstadisticaPartida

        # region ObtenerIdEstadisticaConIdUsuario
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_UsuarioExistente_DeberiaRetornarIdUsuario()
        {
            
            int idUsuario = 1; // Precondición: este ID debe existir en la base de datos y estar relacionado con una estadística.

            
            int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuario);

            
            Assert.IsTrue(resultado > 0, "El método debería devolver el ID del usuario existente.");
        }
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_UsuarioInexistente_DeberiaRetornarMenosUno()
        {
            
            int idUsuarioInexistente = -10; // Precondición: este ID no debe existir en la base de datos.
            //Act 
            int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(idUsuarioInexistente);
            //  Assert
            Assert.AreEqual(resultado, -1, "El método debería devolver -1 para un usuario inexistente.");
        }


        #endregion ObtenerIdEstadisticaConIdUsuario
    }
}
