using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class EstadisticasPartida_Prueba : ConfiguradorPruebaParaServicio
    {
        private const string NOMBRE_JUGADOR_GANADOR = "Jugador1";
        private const int ID_JUGADOR_GANADOR = 1;
        private const string NOMBRE_JUGADOR = "Jugador2";
        private const int ID_JUGADOR = 2;
        private const string NOMBRE_JUGADOR3 = "Jugador3";
        private const int PUNTAJE_GANADOR = 100000;
        private const int PUNTAJE_GENERICO = 10;
        private JugadorEstadisticas jugadorGanador = new JugadorEstadisticas(NOMBRE_JUGADOR_GANADOR, PUNTAJE_GANADOR);
        private JugadorEstadisticas jugadorCasula_1 = new JugadorEstadisticas(NOMBRE_JUGADOR, PUNTAJE_GENERICO);
        private JugadorEstadisticas jugadorCasula_2 = new JugadorEstadisticas(NOMBRE_JUGADOR3, PUNTAJE_GENERICO);

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            imitarEstadisticasDAO
                .Setup(dao => dao.AgregarEstadiscaPartidaAsync(It.IsAny<int>(), It.IsAny<EstadisticasAcciones>(), It.IsAny<int>()))
                .Returns(Task.FromResult(true));
        }

        [TestCleanup]
        public override void LimpiadorDAOs()
        {
            base.LimpiadorDAOs();
        }

        #region GuardarPuntajeAsync
        [TestMethod]
        public async Task GuardarPuntajeAsync_ListaValida_DeberiaGuardarPuntajeCorrectamente()
        {
            
            var estadisticasPartida = new EstadisticasPartida(TematicaPartida.Animales, imitarEstadisticasDAO.Object);
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR_GANADOR, PUNTAJE_GANADOR));
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR, PUNTAJE_GENERICO));

            var listaTuplaNombreIdEstadistica = new List<Tuple<string, int>>
            {
                new Tuple<string, int>(NOMBRE_JUGADOR_GANADOR, ID_JUGADOR_GANADOR),
                new Tuple<string, int>(NOMBRE_JUGADOR, ID_JUGADOR)
            };

            
            await estadisticasPartida.GuardarPuntajeAsync(listaTuplaNombreIdEstadistica);

            
            imitarEstadisticasDAO.Verify(dao =>
                dao.AgregarEstadiscaPartidaAsync(1, EstadisticasAcciones.IncrementarPartidaAnimales, 1), Times.Once, "Debería guardar la victoria para el jugador ganador.");
            imitarEstadisticasDAO.Verify(dao =>
                dao.AgregarEstadiscaPartidaAsync(2, EstadisticasAcciones.IncrementarPartidaAnimales, 0), Times.Once, "Debería guardar la derrota para el jugador perdedor.");
        }



        [TestMethod]
        public async Task GuardarPuntajeAsync_ListaNula_NoDebeRealizarNingunaAccion()
        {
            var estadisticasPartida = new EstadisticasPartida(TematicaPartida.Animales, imitarEstadisticasDAO.Object);

            await estadisticasPartida.GuardarPuntajeAsync(null);

            imitarEstadisticasDAO.Verify(dao => dao.AgregarEstadiscaPartidaAsync(It.IsAny<int>(), It.IsAny<EstadisticasAcciones>(), It.IsAny<int>()), Times.Never, "No se debería llamar a AgregarEstadiscaPartidaAsync con una lista nula.");
            
        }

        [TestMethod]
        public void GuardarPuntajeAsync_CuandoSeLlama_DeberiaLlamarAgregarEstadiscaPartida()
        {
            var tematica = TematicaPartida.Animales;
            var estadisticasPartida = new EstadisticasPartida(tematica, imitarEstadisticasDAO.Object);
            var ejemploJugador100puntos = new JugadorEstadisticas(NOMBRE_JUGADOR_GANADOR);
            ejemploJugador100puntos.Puntos = 100;
            var ejemploJugador50puntos = new JugadorEstadisticas(NOMBRE_JUGADOR);
            ejemploJugador50puntos.Puntos = 50;
            estadisticasPartida.Jugadores.Add(ejemploJugador100puntos);
            estadisticasPartida.Jugadores.Add(ejemploJugador50puntos);

            var listaTuplaNombreIdEstadistica = new List<Tuple<string, int>>
            {
                new Tuple<string, int>(NOMBRE_JUGADOR_GANADOR, ID_JUGADOR_GANADOR),
                new Tuple<string, int>(NOMBRE_JUGADOR, ID_JUGADOR)
            };

            Task.Run(async () =>
            {
                await estadisticasPartida.GuardarPuntajeAsync(listaTuplaNombreIdEstadistica);
            }).GetAwaiter().GetResult();

            imitarEstadisticasDAO.Verify(dao =>
                dao.AgregarEstadiscaPartidaAsync(1, EstadisticasAcciones.IncrementarPartidaAnimales, 1), Times.Once);
            imitarEstadisticasDAO.Verify(dao =>
                dao.AgregarEstadiscaPartidaAsync(2, EstadisticasAcciones.IncrementarPartidaAnimales, 0), Times.Once);
        }
        #endregion GuardarPuntajeAsync

        #region CalcularPodio
        [TestMethod]
        public void CalcularPodio_ListaVacia_DeberiaAsignarPodioANull()
        {
            var estadisticasPartida = new EstadisticasPartida(TematicaPartida.Mixta);

            estadisticasPartida.CalcularPodio();

            Assert.IsNull(estadisticasPartida.PrimerLugar, "Primer lugar debería ser null cuando no hay jugadores.");
            Assert.IsNull(estadisticasPartida.SegundoLugar, "Segundo lugar debería ser null cuando no hay jugadores.");
            Assert.IsNull(estadisticasPartida.TercerLugar, "Tercer lugar debería ser null cuando no hay jugadores.");
        }

        [TestMethod]
        public void CalcularPodio_JugadoresConPuntosIguales_DeberiaOrdenarPorAparicion()
        {
            
            var estadisticasPartida = new EstadisticasPartida(TematicaPartida.Mixta);
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR_GANADOR, 100));
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR3, 100));
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR, 100));

            
            estadisticasPartida.CalcularPodio();

            
            Assert.AreEqual(NOMBRE_JUGADOR_GANADOR, estadisticasPartida.PrimerLugar.Nombre);
            Assert.AreEqual(NOMBRE_JUGADOR3, estadisticasPartida.SegundoLugar.Nombre);
            Assert.AreEqual(NOMBRE_JUGADOR, estadisticasPartida.TercerLugar.Nombre);
        }

        [TestMethod]
        public void CalcularPodio_FaltaUnJugador_DeberiaOrdenarPorAparicion()
        {
            
            var estadisticasPartida = new EstadisticasPartida(TematicaPartida.Mixta);
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR_GANADOR, 100));
            estadisticasPartida.Jugadores.Add(new JugadorEstadisticas(NOMBRE_JUGADOR, 100));

            
            estadisticasPartida.CalcularPodio();

            
            Assert.AreEqual(NOMBRE_JUGADOR_GANADOR, estadisticasPartida.PrimerLugar.Nombre);
            Assert.AreEqual(NOMBRE_JUGADOR, estadisticasPartida.SegundoLugar.Nombre);
        }
        #endregion CalcularPodio
    }
}
