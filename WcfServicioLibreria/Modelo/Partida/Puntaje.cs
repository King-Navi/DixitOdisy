using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    public class Puntaje : IPuntaje
    {
        public const int PUNTOS_ACIERTO = 1;
        public const int PUNTOS_PENALIZACION_NARRADOR = 2;
        public const int PUNTOS_RECIBIDOS_CONFUNDIR = 3;
        public const int MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR = 3;
        public const int PUNTOS_RESTADOS_NO_PARTICIPAR = 1;
        public string NarradorActual {  get; private set; }
        public List<JugadorPuntaje> Jugadores { get; private set; }
        public ConcurrentDictionary<string, List<string>> ImagenesTodosGrupo { get; private set; }
        public ConcurrentDictionary<string, List<string>> ImagenElegidaPorJugador { get; private set; }
        public string ClaveImagenCorrectaActual { get; private set; }
        public bool AlguienAdivino { get; private set; }

        public Puntaje(string narradorActual, 
            List<JugadorPuntaje> jugadores, 
            ConcurrentDictionary<string, List<string>> imagenElegidaPorJugador, 
            ConcurrentDictionary<string, List<string>> imagenPuestasPisina, 
            string claveImagenCorrectaActual)
        {
            NarradorActual = narradorActual;
            Jugadores = jugadores;
            ImagenElegidaPorJugador = imagenElegidaPorJugador;
            ClaveImagenCorrectaActual = claveImagenCorrectaActual;
            ImagenesTodosGrupo = imagenPuestasPisina;
        }

        private List<JugadorPuntaje> ClonarJugadores(List<JugadorPuntaje> jugadoresOriginales)
        {
            return jugadoresOriginales.Select(j => new JugadorPuntaje(j.Nombre)
            {
                Puntos = j.Puntos
            }).ToList();
        }

        public bool CalcularPuntaje()
        {
            var copiaJugadores = ClonarJugadores(Jugadores);
            try
            {
                VerificarAciertos(copiaJugadores, out bool todosAdivinaron, out int votosTotalesCorrectos);
                AplicarPenalizacionNoParticipacion(copiaJugadores);
                EvaluarCondicionesGlobales(copiaJugadores, votosTotalesCorrectos, todosAdivinaron);
                if (AlguienAdivino)
                {
                    AsignarPuntosPorConfundir(copiaJugadores);
                }
                for (int i = 0; i < Jugadores.Count; i++)
                {
                    Jugadores[i].Puntos = copiaJugadores[i].Puntos;
                }
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public void VerificarAciertos(List<JugadorPuntaje> jugadores, out bool todosAdivinaron, out int votosTotalesCorrectos)
        {
            todosAdivinaron = true;
            votosTotalesCorrectos = 0;

            try
            {
                foreach (var jugadorEleccion in ImagenElegidaPorJugador)
                {
                    string nombreJugador = jugadorEleccion.Key;
                    List<string> imagenesSeleccionadas = jugadorEleccion.Value;

                    var jugador = Jugadores.SingleOrDefault(busqueda => busqueda.Nombre == nombreJugador);

                    if (jugador != null)
                    {
                        bool jugadorAdivinoCorrectamente = false;

                        foreach (var imagenId in imagenesSeleccionadas)
                        {
                            if (imagenId.Equals(ClaveImagenCorrectaActual, StringComparison.OrdinalIgnoreCase))
                            {
                                jugador.Puntos += PUNTOS_ACIERTO;
                                votosTotalesCorrectos++;
                                AlguienAdivino = true;
                                jugadorAdivinoCorrectamente = true;
                                Console.WriteLine($"Adivino {jugador.Nombre}");
                                break;
                            }
                        }


                        if (!jugadorAdivinoCorrectamente)
                        {
                            todosAdivinaron = false;
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AplicarPenalizacionNoParticipacion(List<JugadorPuntaje> jugadores)
        {
            try
            {
                foreach (var jugador in Jugadores)
                {
                    if (!jugador.Nombre.Equals(NarradorActual, StringComparison.OrdinalIgnoreCase))
                    {
                        bool noEligioImagen = 
                            !ImagenElegidaPorJugador.TryGetValue(jugador.Nombre, out var imagenesSeleccionadas) 
                            || imagenesSeleccionadas == null 
                            || !imagenesSeleccionadas.Any();
                        bool noPusoImagen = 
                            !ImagenesTodosGrupo.TryGetValue(jugador.Nombre, out var imagenesPuestas)
                            || imagenesPuestas == null 
                            || !imagenesPuestas.Any();
                        if (noEligioImagen || noPusoImagen)
                        {
                            jugador.Puntos -= PUNTOS_RESTADOS_NO_PARTICIPAR;
                            Console.WriteLine($"No participó completamente {jugador.Nombre}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EvaluarCondicionesGlobales(List<JugadorPuntaje> jugadores, int votosCorrectos, bool todosAdivinaron)
        {
            try
            {
                if (todosAdivinaron || votosCorrectos == Jugadores.Count)
                {
                    foreach (var jugador in Jugadores)
                    {
                        if (!jugador.Nombre.Equals(NarradorActual, StringComparison.OrdinalIgnoreCase))
                        {
                            jugador.Puntos += PUNTOS_PENALIZACION_NARRADOR;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AsignarPuntosPorConfundir(List<JugadorPuntaje> jugadores)
        {
            try
            {
                foreach (var jugadorPusoImagen in ImagenesTodosGrupo)
                {
                    string nombreJugador = jugadorPusoImagen.Key;
                    List<string> imagenesPuestas = jugadorPusoImagen.Value; 
                    if (nombreJugador.Equals(NarradorActual, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var jugador = Jugadores.SingleOrDefault(busqueda => busqueda.Nombre.Equals(nombreJugador, StringComparison.OrdinalIgnoreCase));

                    if (jugador != null)
                    {
                        int jugadoresConfundidos = 0;
                        HashSet<string> jugadoresYaContados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        foreach (string imagenClave in imagenesPuestas)
                        {
                            IEnumerable<string> jugadoresQueEligieronEstaImagen = ImagenElegidaPorJugador
                                .Where(busqueda => !busqueda.Key.Equals(nombreJugador, StringComparison.OrdinalIgnoreCase)) 
                                .Where(busqueda => busqueda.Value.Contains(imagenClave, StringComparer.OrdinalIgnoreCase)) 
                                .Select(busqueda => busqueda.Key)
                                .Distinct();

                            foreach (string jugadorConfundido in jugadoresQueEligieronEstaImagen)
                            {
                                if (jugadoresYaContados.Add(jugadorConfundido))
                                {
                                    jugadoresConfundidos++;
                                    if (jugadoresConfundidos >= MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (jugadoresConfundidos >= MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR)
                            {
                                break;
                            }
                        }
                        int puntosPorConfundir = jugadoresConfundidos * PUNTOS_RECIBIDOS_CONFUNDIR;
                        if (puntosPorConfundir > MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR * PUNTOS_RECIBIDOS_CONFUNDIR)
                        {
                            puntosPorConfundir = MAXIMO_VECES_RECIBIR_PUNTOS_CONFUNDIR * PUNTOS_RECIBIDOS_CONFUNDIR;
                        }
                        jugador.Puntos += puntosPorConfundir;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
