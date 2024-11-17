using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO
{
    [TestClass]
    public class EstadisticasDAO_Prueba : ConfiguracionPruebaBD
    {
        #region RecuperarEstadisticas
        [TestMethod]
        public void RecuperarEstadisticas_IdExistente_DeberiaRetornarEstadistica()
        {
            // Arrange
            //Precondicon: debe existir el id en base de datos
            int idEstadisticas = 1;

            // Act
            var resultado = DAOLibreria.DAO.EstadisticasDAO.RecuperarEstadisticas(idEstadisticas);

            // Assert
            Assert.IsNotNull(resultado, "El resultado no debería ser nulo.");
            Assert.AreEqual(idEstadisticas, resultado.idEstadisticas, "El ID de la estadística no coincide.");
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoValido_DeberiaRetornarExcepcion()
        {
            // Arrange
            int idEstadisticas = -1;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => DAOLibreria.DAO.EstadisticasDAO.RecuperarEstadisticas(idEstadisticas), "Deber retornar un argument exceotion");
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoExiste_DeberiaRetornarExcepcion()
        {
            // Arrange
            //Precondicon: NO debe existir el id en base de datos
            int idEstadisticas = 2030;

            // Act & Assert

            Assert.ThrowsException<ArgumentException>(() => DAOLibreria.DAO.EstadisticasDAO.RecuperarEstadisticas(idEstadisticas), "Deber retornar un argument exceotion");
        }
        #endregion RecuperarEstadisticas

        #region AgregarEstadisticaPartida

        [TestMethod]
        public async Task AgregarEstadisticaPartida_IdExistente_DeberiaActualizarEstadisticas()
        {
            // Arrange
            //Precondcion: debe existir el id en base de datos
            int idEstadisticas = 1;
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;
            int victoria = 1;

            // Act
            var resultado = await EstadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria);

            // Assert
            Assert.IsTrue(resultado, "El resultado debería ser verdadero.");
        }
        [TestMethod]
        public async Task AgregarEstadisticaPartida_MultiplesAcciones_DeberiaActualizarEstadisticas()
        {
            // Arrange
            //Precondicon: debe existir el id en base de datos
            int idEstadisticas = 1;
            int victoria = 1;
            var estadisticasAnteriores = EstadisticasDAO.RecuperarEstadisticas(idEstadisticas);
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

            // Act
            var tareasSolicitudes = new List<Task>();
            foreach (var accion in acciones)
            {
                tareasSolicitudes.Add(EstadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria));
            }
            await Task.WhenAll(tareasSolicitudes);

            var estadisticasNuevas = EstadisticasDAO.RecuperarEstadisticas(idEstadisticas);

            // Assert
            Assert.IsTrue(estadisticasAnteriores.partidasGanadas + acciones.Count == estadisticasNuevas.partidasGanadas, "El número de partidas ganadas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.partidasJugadas + acciones.Count == estadisticasNuevas.partidasJugadas, "El número de partidas jugadas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.vecesTematicaMixto + 1 == estadisticasNuevas.vecesTematicaMixto, "El número de partidas mixtas no coincide.");
            Assert.IsTrue(estadisticasAnteriores.vecesTematicaAnimales + 1 == estadisticasNuevas.vecesTematicaAnimales, "El número de partidas mixtas no coincide.");

        }

        [TestMethod]
        public async Task AgregarEstadisticaPartida_VictoriaMayor_DeberiaRetornarExcepcion()
        {
            // Arrange
            //Precondcion: debe existir el id en base de datos
            int idEstadisticas = 1;
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;
            int victoria = 2;


            // Act & Assert
            await Assert.ThrowsExceptionAsync<ActividadSospechosaExcepcion>(async () => await EstadisticasDAO.AgregarEstadiscaPartidaAsync(idEstadisticas, accion, victoria), "Deber retornar un actividad sospechosa exceotion");
        }
        #endregion AgregarEstadisticaPartida


        public static int ObtenerIdEstadisticaConIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Estadisticas.SingleOrDefault(fila => fila.idUsuario == idUsuario);

                    if (usuario == null)
                    {
                        throw new ArgumentException();
                    }

                    return usuario.idUsuario;
                }
            }
            catch (Exception)
            {
            }
            return -1;
        }
        #region ObtenerIdEstadisticaConIdUsuario
        

        #endregion ObtenerIdEstadisticaConIdUsuario
        #region
        #endregion
        #region
        #endregion
    }
}
