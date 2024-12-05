using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class Puntaje_Prueba
    {
        private const string JUGADOR_1 = "Jugador1";
        private const string JUGADOR_2 = "Jugador2";
        private const string JUGADOR_3 = "Jugador3";
        private const string JUGADOR_4 = "Jugador4";
        private const string JUGADOR_5 = "Jugador5";
        private const string JUGADOR_6 = "Jugador6";
        private const string NARRADOR = "Narrador";
        private const string IMAGEN_CORRECTA = "ImagenCorrecta";
        private const string IMAGEN_INCORRECTA = "ImagenIncorrecta";
        private const string IMAGEN_A = "ImagenA";
        private const string IMAGEN_B = "ImagenB";
        private const string IMAGEN_C = "ImagenC";
        private const string IMAGEN_D = "ImagenD";
        private const string IMAGEN_E = "ImagenE";
        private const string IMAGEN_F = "ImagenF";
        private const string IMAGEN_X = "ImagenX";
        private const string IMAGEN_Y = "ImagenY";
        private const string IMAGEN_Z = "ImagenZ";


        private void ImprimirPuntajes(List<JugadorEstadisticas> jugadores)
        {
            Console.WriteLine("-----Puntajes de los jugadores-----");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.Nombre}, Puntos: {jugador.Puntos}");
            }
            Console.WriteLine("--------------------------------------------");
        }
        #region CalcularPuntaje

        [TestMethod]
        public void CalcularPuntaje_SiOcurreExcepcionTodoNulo_NoModificaPuntajes()
        {
            // Arrange
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1) { Puntos = 5 },
                new JugadorEstadisticas(JUGADOR_2) { Puntos = 3 }
            };
            var puntaje = new Puntaje(JUGADOR_2, jugadores, null, null, null);
            ImprimirPuntajes(jugadores);
            bool exito = puntaje.CalcularPuntaje();
            ImprimirPuntajes(jugadores);
            Assert.IsFalse(exito, "El cálculo debería fallar debido a la excepción.");
            Assert.AreEqual(5, jugadores[0].Puntos, "El puntaje del jugador1 no debería cambiar.");
            Assert.AreEqual(3, jugadores[1].Puntos, "El puntaje del jugador2 no debería cambiar.");
        }

        [TestMethod]
        public void CalcularPuntaje_SiOcurreExcepcion_NoModificaPuntajes()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1) { Puntos = 5 },
                new JugadorEstadisticas(JUGADOR_2) { Puntos = 3 }
            };
            var puntaje = new Puntaje(JUGADOR_2, jugadores, new ConcurrentDictionary<string, List<string>>(), null, null);
            ImprimirPuntajes(jugadores);
            bool exito = puntaje.CalcularPuntaje();
            ImprimirPuntajes(jugadores);
            Assert.IsFalse(exito, "El cálculo debería fallar debido a la excepción.");
            Assert.AreEqual(5, jugadores[0].Puntos, "El puntaje del jugador1 no debería cambiar.");
            Assert.AreEqual(3, jugadores[1].Puntos, "El puntaje del jugador2 no debería cambiar.");
        }

        #endregion CalcularPuntaje


        #region VerificarAciertos
        [TestMethod]
        public void VerificarAciertos_CuandoJugadorAdivinaCorrectamente_DeberiaAsignarPuntosYRegistrarAcierto()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2)
            };

            var imagenesPorJugador = new ConcurrentDictionary<string, List<string>>
            {
                [JUGADOR_1] = new List<string> { IMAGEN_CORRECTA },
                [JUGADOR_2] = new List<string> { IMAGEN_INCORRECTA },
                [NARRADOR] = new List<string> { IMAGEN_CORRECTA }
            };

            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesPorJugador, null, IMAGEN_CORRECTA);
            ImprimirPuntajes(puntaje.Jugadores);
            puntaje.VerificarAciertos(jugadores,out var todosAdivinaron, out var votosCorrectos);
            ImprimirPuntajes(puntaje.Jugadores);
            Assert.IsTrue(puntaje.AlguienAdivino, "Debería registrarse que alguien adivinó.");
            Assert.AreEqual(1, votosCorrectos, "Debería contar un voto correcto.");
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugadores[0].Puntos, "Debería asignar puntos al jugador que adivinó.");
            Assert.AreEqual(0, jugadores[1].Puntos, "El jugador que no adivinó no debería recibir puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_TodosAdivinanCorrectamente_DeberiaAsignarPuntosYRegistrarTodosAdivinaron()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(NARRADOR)
            };

            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_CORRECTA },
                [JUGADOR_2] = new List<string> { IMAGEN_CORRECTA }
            };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsTrue(puntaje.AlguienAdivino, "AlguienAdivino debería ser true.");
            Assert.IsTrue(todosAdivinaron, "todosAdivinaron debería ser true.");
            Assert.AreEqual(2, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 2.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(j => j.Nombre == JUGADOR_2);
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugador1.Puntos, $"{JUGADOR_1} debería tener {Puntaje.PUNTOS_ACIERTO} puntos.");
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugador2.Puntos, $"{JUGADOR_2} debería tener {Puntaje.PUNTOS_ACIERTO} puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_NingunJugadorAdivina_DeberiaNoAsignarPuntosYRegistrarNadieAdivino()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(NARRADOR)
            };

            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_INCORRECTA },
                [JUGADOR_2] = new List<string> { IMAGEN_INCORRECTA }
            };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsFalse(puntaje.AlguienAdivino, "AlguienAdivino debería ser false.");
            Assert.IsFalse(todosAdivinaron, "todosAdivinaron debería ser false.");
            Assert.AreEqual(0, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 0.");
            var jugador1 = jugadores.Single(busqueda    => busqueda.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_2);
            Assert.AreEqual(0, jugador1.Puntos, $"{JUGADOR_1} debería tener 0 puntos.");
            Assert.AreEqual(0, jugador2.Puntos, $"{JUGADOR_2} debería tener 0 puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_UnJugadorAdivinaOtroNo_DeberiaAsignarPuntosYRegistrarParcialmente()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_CORRECTA },
                [JUGADOR_2] = new List<string> { IMAGEN_INCORRECTA }
            };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsTrue(puntaje.AlguienAdivino, "AlguienAdivino debería ser true.");
            Assert.IsFalse(todosAdivinaron, "todosAdivinaron debería ser false.");
            Assert.AreEqual(1, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 1.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(j => j.Nombre == JUGADOR_2);
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugador1.Puntos, $"{JUGADOR_1} debería tener {Puntaje.PUNTOS_ACIERTO} puntos.");
            Assert.AreEqual(0, jugador2.Puntos, $"{JUGADOR_2} debería tener 0 puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_JugadorSeleccionaVariasImagenesUnaEsCorrecta_DeberiaAsignarPuntos()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };

            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_INCORRECTA, IMAGEN_CORRECTA }
            };

            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsTrue(puntaje.AlguienAdivino, "AlguienAdivino debería ser true.");
            Assert.IsTrue(todosAdivinaron, "todosAdivinaron debería ser true."); 
            Assert.AreEqual(1, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 1.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugador1.Puntos, $"{JUGADOR_1} debería tener {Puntaje.PUNTOS_ACIERTO} puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_JugadorSeleccionaVariasImagenes_NingunaEsCorrecta_NoDeberiaAsignarPuntos()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_INCORRECTA, IMAGEN_A }
            };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsFalse(puntaje.AlguienAdivino, "AlguienAdivino debería ser false.");
            Assert.IsFalse(todosAdivinaron, "todosAdivinaron debería ser false.");
            Assert.AreEqual(0, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 0.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(0, jugador1.Puntos, $"{JUGADOR_1} debería tener 0 puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_JugadorNoEligioImagen_NoDeberiaAsignarPuntos()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA);
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsFalse(puntaje.AlguienAdivino, "AlguienAdivino debería ser false.");
            Assert.IsTrue(todosAdivinaron, "todosAdivinaron debería ser true.");
            Assert.AreEqual(0, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 0.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(0, jugador1.Puntos, $"{JUGADOR_1} debería tener 0 puntos.");
        }

        [TestMethod]
        public void VerificarAciertos_JugadorEligeImagenConMayusculas_DeberiaAsignarPuntos()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_CORRECTA.ToUpper() }
            };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, null, IMAGEN_CORRECTA.ToLower());
            puntaje.VerificarAciertos(jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
            Assert.IsTrue(puntaje.AlguienAdivino, "AlguienAdivino debería ser true.");
            Assert.IsTrue(todosAdivinaron, "todosAdivinaron debería ser true.");
            Assert.AreEqual(1, votosTotalesCorrectos, "votosTotalesCorrectos debería ser 1.");
            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(Puntaje.PUNTOS_ACIERTO, jugador1.Puntos, $"{JUGADOR_1} debería tener {Puntaje.PUNTOS_ACIERTO} puntos.");
        }


        #endregion VerificarAciertos

        #region AplicarPenalizacionNoParticipacion
        [TestMethod]
        public void AplicarPenalizacionNoParticipacion_JugadorNoEligioImagen_DeberiaPenalizar()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            imagenesTodosGrupo[JUGADOR_1] = new List<string> { IMAGEN_A };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AplicarPenalizacionNoParticipacion(jugadores);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            Assert.AreEqual(-Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR, jugador1.Puntos, $"{JUGADOR_1} debería ser penalizado por no elegir una imagen al adivinar.");
        }

        [TestMethod]
        public void AplicarPenalizacionNoParticipacion_JugadorNoColocoImagen_DeberiaPenalizar()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            imagenesElegidasPorJugador[JUGADOR_1] = new List<string> { IMAGEN_X }; 
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AplicarPenalizacionNoParticipacion(jugadores);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            Assert.AreEqual(-Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR, jugador1.Puntos, $"{JUGADOR_1} debería ser penalizado por no colocar una imagen en la piscina.");
        }
        [TestMethod]
        public void AplicarPenalizacionNoParticipacion_JugadorNoParticipoEnNingunaAccion_DeberiaPenalizar()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AplicarPenalizacionNoParticipacion(jugadores);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            Assert.AreEqual(-Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR, jugador1.Puntos, $"{JUGADOR_1} debería ser penalizado por no participar en ninguna acción.");
        }

        [TestMethod]
        public void AplicarPenalizacionNoParticipacion_NarradorNoEsPenalizado()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AplicarPenalizacionNoParticipacion(jugadores);
            var narrador = jugadores.Single(busqueda => busqueda.Nombre == NARRADOR);
            Assert.AreEqual(0, narrador.Puntos, "El narrador no debería ser penalizado.");
        }

        [TestMethod]
        public void AplicarPenalizacionNoParticipacion_MultiplesJugadoresAlgunosNoParticipan_CorrectaPenalizacion()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3),
                new JugadorEstadisticas(NARRADOR)
            };
            var imagenesElegidasPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            imagenesElegidasPorJugador[JUGADOR_1] = new List<string> { IMAGEN_X }; 
            imagenesElegidasPorJugador[JUGADOR_3] = new List<string> { IMAGEN_Y };
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            imagenesTodosGrupo[JUGADOR_1] = new List<string> { IMAGEN_A };
            imagenesTodosGrupo[JUGADOR_2] = new List<string> { IMAGEN_B };
            var puntaje = new Puntaje(NARRADOR, jugadores, imagenesElegidasPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AplicarPenalizacionNoParticipacion(jugadores);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_2);
            var jugador3 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_3);
            Assert.AreEqual(0, jugador1.Puntos, $"{JUGADOR_1} no debería ser penalizado.");
            Assert.AreEqual(-Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR, jugador2.Puntos, $"{JUGADOR_2} debería ser penalizado por no elegir una imagen al adivinar.");
            Assert.AreEqual(-Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR, jugador3.Puntos, $"{JUGADOR_3} debería ser penalizado por no colocar una imagen en la piscina.");
        }

        #endregion AplicarPenalizacionNoParticipacion

        #region EvaluarCondicionesGlobales
        [TestMethod]
        public void EvaluarCondicionesGlobales_TodosAdivinaron_AsignarPuntosANoNarradores()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(NARRADOR)
            };
            int votosCorrectos = 2;
            bool todosAdivinaron = true;
            var puntaje = new Puntaje(NARRADOR, jugadores, null, null, IMAGEN_CORRECTA);
            puntaje.EvaluarCondicionesGlobales(jugadores, votosCorrectos, todosAdivinaron);
            var narrador = jugadores.Single(busqueda => busqueda.Nombre == NARRADOR);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_2);
            Assert.AreEqual(0, narrador.Puntos, "El narrador no debería recibir puntos.");
            Assert.AreEqual(Puntaje.PUNTOS_PENALIZACION_NARRADOR, jugador1.Puntos, $"{JUGADOR_1} debería recibir puntos de penalización al narrador.");
            Assert.AreEqual(Puntaje.PUNTOS_PENALIZACION_NARRADOR, jugador2.Puntos, $"{JUGADOR_2} debería recibir puntos de penalización al narrador.");
        }

        [TestMethod]
        public void EvaluarCondicionesGlobales_VotosCorrectosIgualNumeroJugadores_AsignarPuntosANoNarradores()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(NARRADOR)
            };
            int votosCorrectos = 2;
            bool todosAdivinaron = false;
            var puntaje = new Puntaje(NARRADOR, jugadores, null, null, IMAGEN_CORRECTA);
            puntaje.EvaluarCondicionesGlobales(jugadores, votosCorrectos, todosAdivinaron);
            var narrador = jugadores.Single(busqueda => busqueda.Nombre == NARRADOR);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            Assert.AreEqual(0, narrador.Puntos, "El narrador no debería recibir puntos.");
            Assert.AreEqual(Puntaje.PUNTOS_PENALIZACION_NARRADOR, jugador1.Puntos, $"{JUGADOR_1} debería recibir puntos de penalización al narrador.");
        }
        [TestMethod]
        public void EvaluarCondicionesGlobales_NingunaCondicionCumplida_NoAsignarPuntos()
        {
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(NARRADOR)
            };
            int votosCorrectos = 1; 
            bool todosAdivinaron = false;
            var puntaje = new Puntaje(NARRADOR, jugadores, null, null, IMAGEN_CORRECTA);
            puntaje.EvaluarCondicionesGlobales(jugadores, votosCorrectos, todosAdivinaron);
            foreach (var jugador in jugadores)
            {
                Assert.AreEqual(0, jugador.Puntos, $"{jugador.Nombre} no debería recibir puntos adicionales.");
            }
        }


        #endregion EvaluarCondicionesGlobales


        #region AsignarPuntosPorConfundir
        [TestMethod]
        public void AsignarPuntosPorConfundir_UnJugadorConfundeAUno_DeberiaAumentar()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase);
            imagenesTodosGrupo[JUGADOR_1] = new List<string> { IMAGEN_A };
            imagenesTodosGrupo[JUGADOR_2] = new List<string> { IMAGEN_B };
            imagenesTodosGrupo[JUGADOR_3] = new List<string> { IMAGEN_C };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase);
            imagenElegidaPorJugador[JUGADOR_1] = new List<string> { IMAGEN_X };
            imagenElegidaPorJugador[JUGADOR_2] = new List<string> { IMAGEN_A };
            imagenElegidaPorJugador[JUGADOR_3] = new List<string> { IMAGEN_Y };
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3)
            };
            string NarradorActual = JUGADOR_3;
            var puntaje = new Puntaje(NarradorActual, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            var jugadorEstadisticas = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR, jugadorEstadisticas.Puntos, "Jugador1 debería tener puntos por confundir a Jugador2.");
        }


        [TestMethod]
        public void AsignarPuntosPorConfundir_UnJugadorConfundeATresJugadores_DeberiaAumentarAlMaximo()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase);
            imagenesTodosGrupo[JUGADOR_1] = new List<string> { IMAGEN_A };
            imagenesTodosGrupo[JUGADOR_2] = new List<string> { IMAGEN_B };
            imagenesTodosGrupo[JUGADOR_3] = new List<string> { IMAGEN_C };
            imagenesTodosGrupo[JUGADOR_4] = new List<string> { IMAGEN_D };
            imagenesTodosGrupo[JUGADOR_5] = new List<string> { IMAGEN_E };
            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(System.StringComparer.OrdinalIgnoreCase);
            imagenElegidaPorJugador[JUGADOR_2] = new List<string> { IMAGEN_A };
            imagenElegidaPorJugador[JUGADOR_3] = new List<string> { IMAGEN_A };
            imagenElegidaPorJugador[JUGADOR_4] = new List<string> { IMAGEN_A };
            imagenElegidaPorJugador[JUGADOR_5] = new List<string> { IMAGEN_F };
            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3),
                new JugadorEstadisticas(JUGADOR_4),
                new JugadorEstadisticas(JUGADOR_5)
            };
            string NarradorActual = JUGADOR_5;
            var puntaje = new Puntaje(NarradorActual, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            var jugadorQueConfundio = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            var resultado = Puntaje.MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR * Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR;
            Assert.AreEqual(resultado, jugadorQueConfundio.Puntos, $"{JUGADOR_1} debería tener 3 puntos por confundir a tres jugadores (máximo permitido).");
        }
        [TestMethod]
        public void AsignarPuntosPorConfundir_JugadorConfundeAMasDeTresJugadores_DeberiaLimitarPuntosAlMaximo()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A }
            };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_2] = new List<string> { IMAGEN_A },
                [JUGADOR_3] = new List<string> { IMAGEN_A },
                [JUGADOR_4] = new List<string> { IMAGEN_A },
                [JUGADOR_5] = new List<string> { IMAGEN_A },
                [JUGADOR_6] = new List<string> { IMAGEN_A }
            };

            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3),
                new JugadorEstadisticas(JUGADOR_4),
                new JugadorEstadisticas(JUGADOR_5),
                new JugadorEstadisticas(JUGADOR_6)
            };


            var puntaje = new Puntaje(null, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            ImprimirPuntajes(puntaje.Jugadores);
            var jugadorQueConfundio = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            var puntosEsperados = Puntaje.MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR * Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR;
            Assert.AreEqual(puntosEsperados, jugadorQueConfundio.Puntos, $"{JUGADOR_1} debería tener {puntosEsperados} puntos por confundir a más de tres jugadores (máximo permitido).");
        }


        [TestMethod]
        public void AsignarPuntosPorConfundir_NarradorVista_NoRecibePuntosPorConfundir()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [NARRADOR] = new List<string> { IMAGEN_A },
                [JUGADOR_1] = new List<string> { IMAGEN_B },
                [JUGADOR_2] = new List<string> { IMAGEN_C }
            };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A },
                [JUGADOR_2] = new List<string> { IMAGEN_B },
                [NARRADOR] = new List<string> { IMAGEN_A }

            };

            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(NARRADOR),
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2)
            };

            var puntaje = new Puntaje(NARRADOR, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_A);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            ImprimirPuntajes(puntaje.Jugadores);
            var narrador = jugadores.Single(busqueda => busqueda.Nombre == NARRADOR);
            Assert.AreEqual(0, narrador.Puntos, "El narrador no debería recibir puntos por confundir.");
        }
        [TestMethod]
        public void AsignarPuntosPorConfundir_JugadorNoSeConfundeASiMismo_NoDeberiaSumarPuntos()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A },
                [JUGADOR_2] = new List<string> { IMAGEN_B }
            };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A },
                [JUGADOR_2] = new List<string> { IMAGEN_B }
            };

            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2)
            };

            string NarradorActual = NARRADOR;

            var puntaje = new Puntaje(NarradorActual, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            ImprimirPuntajes(puntaje.Jugadores);
            var jugadorEvaluacion = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            Assert.AreEqual(0, jugadorEvaluacion.Puntos, $"{JUGADOR_1} no debería recibir puntos por confundirse a sí mismo.");
        }

        [TestMethod]
        public void AsignarPuntosPorConfundir_VariosJugadoresConfundenAlMismoJugador()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A },
                [JUGADOR_2] = new List<string> { IMAGEN_B },
                [JUGADOR_3] = new List<string> { IMAGEN_C }
            };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_4] = new List<string> { IMAGEN_A, IMAGEN_B },
            };

            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3),
                new JugadorEstadisticas(JUGADOR_4)
            };

            string NarradorActual = JUGADOR_3;

            var puntaje = new Puntaje(NarradorActual, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            ImprimirPuntajes(puntaje.Jugadores);
            var jugador1 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_1);
            var jugador2 = jugadores.Single(busqueda => busqueda.Nombre == JUGADOR_2);

            Assert.AreEqual(Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR, jugador1.Puntos, $"{JUGADOR_1} debería recibir puntos por confundir a {JUGADOR_4}.");
            Assert.AreEqual(Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR, jugador2.Puntos, $"{JUGADOR_2} debería recibir puntos por confundir a {JUGADOR_4}.");
        }

        [TestMethod]
        public void AsignarPuntosPorConfundir_JugadorNoPusoImagen_NoRecibePuntos()
        {
            var imagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_1] = new List<string> { IMAGEN_A },
                [JUGADOR_3] = new List<string> { IMAGEN_C }
            };

            var imagenElegidaPorJugador = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [JUGADOR_4] = new List<string> { IMAGEN_A, IMAGEN_B },
            };

            var jugadores = new List<JugadorEstadisticas>
            {
                new JugadorEstadisticas(JUGADOR_1),
                new JugadorEstadisticas(JUGADOR_2),
                new JugadorEstadisticas(JUGADOR_3),
                new JugadorEstadisticas(JUGADOR_4)
            };

            string NarradorActual = JUGADOR_3;

            var puntaje = new Puntaje(NarradorActual, jugadores, imagenElegidaPorJugador, imagenesTodosGrupo, IMAGEN_CORRECTA);
            puntaje.AsignarPuntosPorConfundir(jugadores);
            ImprimirPuntajes(puntaje.Jugadores);
            var jugador2 = jugadores.Single(j => j.Nombre == JUGADOR_2);
            Assert.AreEqual(0, jugador2.Puntos, $"{JUGADOR_2} no debería recibir puntos al no poner ninguna imagen.");

            var jugador1 = jugadores.Single(j => j.Nombre == JUGADOR_1);
            Assert.AreEqual(Puntaje.PUNTOS_RECIBIDOS_CONFUNDIR, jugador1.Puntos, $"{JUGADOR_1} debería recibir puntos por confundir a {JUGADOR_4}.");
        }
        #endregion AsignarPuntosPorConfundir


    }



}
