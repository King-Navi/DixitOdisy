using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
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
            var resultado = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.IsNotNull(resultado, "El resultado no debería ser nulo.");
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoValido_DeberiaRetornarNull()
        {
            var resultado = estadisticasDAO.RecuperarEstadisticas(ID_INVALIDO);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void RecuperarEstadisticas_IdNoExiste_DeberiaRetornarNull()
        {
            var resultado = estadisticasDAO.RecuperarEstadisticas(ID_INEXISTENTE);
            Assert.IsNull(resultado);
        }
        #endregion RecuperarEstadisticas

        #region AgregarEstadisticaPartida

        [TestMethod]
        public async Task AgregarEstadisticaPartida_IdExistente_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;

            var resultado = await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);

            Assert.IsTrue(resultado, "El resultado debería ser verdadero.");
        }

        [TestMethod]
        public async Task IncrementarPartidaMixta_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.vecesTematicaMixto + INCREMENTO_MAXIMO,
                estadisticasNuevas.vecesTematicaMixto,
                "El número de partidas mixtas aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidaEspacio_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaEspacio;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.vecesTematicaEspacio + INCREMENTO_MAXIMO,
                estadisticasNuevas.vecesTematicaEspacio,
                "El número de partidas de espacio aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidaPaises_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaPaises;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.vecesTematicaPaises + INCREMENTO_MAXIMO,
                estadisticasNuevas.vecesTematicaPaises,
                "El número de partidas de países aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidaAnimales_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaAnimales;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.vecesTematicaAnimales + INCREMENTO_MAXIMO,
                estadisticasNuevas.vecesTematicaAnimales,
                "El número de partidas de animales aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidaMitologia_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidasMitologia;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.vecesTematicaMitologia + INCREMENTO_MAXIMO,
                estadisticasNuevas.vecesTematicaMitologia,
                "El número de partidas de mitología aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidasJugadas_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.partidasJugadas + INCREMENTO_MAXIMO,
                estadisticasNuevas.partidasJugadas,
                "El número de partidas jugadas aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task IncrementarPartidasGanadas_DeberiaActualizarEstadisticas()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;

            var estadisticasAnteriores = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_MAXIMO);
            var estadisticasNuevas = estadisticasDAO.RecuperarEstadisticas(ID_VALIDO);

            Assert.AreEqual(
                estadisticasAnteriores.partidasGanadas + INCREMENTO_MAXIMO,
                estadisticasNuevas.partidasGanadas,
                "El número de partidas ganadas aumentó exactamente 1 vez."
            );
        }

        [TestMethod]
        public async Task AgregarEstadisticaPartida_VictoriaMayor_DeberiaRetornarExcepcion()
        {
            EstadisticasAcciones accion = EstadisticasAcciones.IncrementarPartidaMixta;

            await Assert.ThrowsExceptionAsync<ActividadSospechosaExcepcion>(async () => await estadisticasDAO.AgregarEstadiscaPartidaAsync(ID_VALIDO, accion, INCREMENTO_INVALIDO), "Deber retornar un actividad sospechosa exception");
        }
        #endregion AgregarEstadisticaPartida

        # region ObtenerIdEstadisticaConIdUsuario
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_UsuarioExistente_DeberiaRetornarIdUsuario()
        {
            int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(ID_VALIDO);

            Assert.IsTrue(resultado > 0, "El método debería devolver el ID del usuario existente.");
        }
        [TestMethod]
        public void ObtenerIdEstadisticaConIdUsuario_UsuarioInexistente_DeberiaRetornarMenosUno()
        {
            int resultado = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(ID_INEXISTENTE);
            
            Assert.AreEqual(resultado, -1, "El método debería devolver -1 para un usuario inexistente.");
        }
        #endregion ObtenerIdEstadisticaConIdUsuario
    }
}
