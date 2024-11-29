using DAOLibreria.DAO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    internal partial class Partida
    {

        #region Ronda
        public async Task EmpezarPartida()
        {
            await semaphoreEmpezarPartida.WaitAsync();
            try
            {
                if (SeLlamoEmpezarPartida)
                {
                    return;
                }

                DateTime inicioEspera = DateTime.Now;

                while ((DateTime.Now - inicioEspera).TotalSeconds < TIEMPO_ESPERA_UNIRSE_JUGADORES)
                {
                    await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
                }
                SeTerminoEsperaUnirse = true;
                if (!SeLlamoEmpezarPartida && jugadoresCallback.Count >= CANTIDAD_MINIMA_JUGADORES)
                {
                    SeLlamoEmpezarPartida = true;
                    TodosListos?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await TerminarPartidaAsync();
                }
            }
            finally
            {
                semaphoreEmpezarPartida.Release();
            }
        }

        private async Task IniciarPartidaSeguroAsync(CancellationToken cancelacionToke)
        {
            try
            {
                estadisticasPartida.AgregarDesdeOtraLista(ObtenerNombresJugadores());
                await EjecutarRondasAsync(cancelacionToke);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar ronda {RondaActual} :  {ex.Message} ");
            }
        }

        private async Task EjecutarRondasAsync(CancellationToken cancelacionToke)
        {
            try
            {
                while (!VerificarCondicionVictoria() && ContarJugadores() >= CANTIDAD_MINIMA_JUGADORES && !cancelacionToke.IsCancellationRequested)
                {
                    Console.WriteLine("Ronda: " + RondaActual);
                    await EjecutarRondaAsync();
                    await EvaluarPuntosRondaAsync();
                    CambiarPantalla(PANTALLA_INICIO);
                    ++RondaActual;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("La tarea fue cancelada.");
            }
            catch (Exception)
            {
            }
            await TerminarPartidaAsync();
        }

        private async Task EvaluarPuntosRondaAsync()  //FIXME
        {
            CalculoPuntosEnSituaciones();
            estadisticasPartida.CalcularPodio();
            try
            {
                foreach (var nombre in ObtenerNombresJugadores().ToList())
                {

                    lock (jugadoresCallback)
                    {
                        jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback);
                        callback.EnviarEstadisticas(estadisticasPartida);
                    }

                }
            }
            catch (Exception)
            {
            }
            CambiarPantalla(PANTALLA_ESTADISTICAS);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_MOSTRAR_ESTADISTICAS));

        }

        //TODO:
        //Caso en el que todos adivinan la imagen correcta. CHECK
        //Caso en el que nadie adivina la imagen correcta. CHECK
        //Caso en el que algunos adivinan y otros no. CHECK
        //Caso en el que uno o varios jugadores no seleccionan ninguna imagen. CHECK
        private void CalculoPuntosEnSituaciones()
        {

            // 1. Verificar quiénes adivinaron correctamente
            VerificarAciertos(out bool alguienAdivinoImagen, out bool todosAdivinaron, out int votosCorrectos);

            // 2. Aplicar penalización a los jugadores que no participaron
            AplicarPenalizacionNoParticipacion();

            // 3. Asignar puntos de penalización o bonificación si todos/nadie acertó
            EvaluarCondicionesGlobales(votosCorrectos, todosAdivinaron);

            // 4. Asignar puntos de confusión por votos de otros jugadores
            AsignarPuntosPorConfundir();
        }

        private void VerificarAciertos(out bool alguienAdivinoImagen, out bool todosAdivinaron, out int votosCorrectos)
        {
            alguienAdivinoImagen = false;
            todosAdivinaron = true;
            votosCorrectos = 0;

            foreach (var jugadorEleccion in JugadorImagenElegida)
            {
                string nombreJugador = jugadorEleccion.Key;
                List<string> imagenesSeleccionadas = jugadorEleccion.Value;

                var jugador = estadisticasPartida.Jugadores.SingleOrDefault(j => j.Nombre == nombreJugador);

                if (jugador != null)
                {
                    bool jugadorAdivinoCorrectamente = false;

                    foreach (var imagenId in imagenesSeleccionadas)
                    {
                        if (imagenId == ClaveImagenCorrectaActual)
                        {
                            jugador.Puntos += PUNTOS_ACIERTO;
                            votosCorrectos++;
                            alguienAdivinoImagen = true;
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

        private void AplicarPenalizacionNoParticipacion()
        {
            foreach (var jugador in estadisticasPartida.Jugadores)
            {
                if (jugador.Nombre != NarradorActual
                    && JugadorImagenElegida.TryGetValue(jugador.Nombre, out var imagenesSeleccionadas)
                    && (imagenesSeleccionadas == null || !imagenesSeleccionadas.Any()))
                {
                    jugador.Puntos -= PUNTOS_RESTADOS_NO_PARTICIPAR;
                    Console.WriteLine($"No participo {jugador.Nombre}");
                }
            }
        }

        private void EvaluarCondicionesGlobales(int votosCorrectos, bool todosAdivinaron)
        {
            if (votosCorrectos == NUM_JUGADOR_NADIE_ACERTO || votosCorrectos == estadisticasPartida.Jugadores.Count)
            {
                Console.WriteLine("Todos o nadie acertó");
                foreach (var jugador in estadisticasPartida.Jugadores)
                {
                    if (jugador.Nombre != NarradorActual)
                    {
                        jugador.Puntos += PUNTOS_PENALIZACION_NARRADOR;
                    }
                }
            }
        }

        private void AsignarPuntosPorConfundir()
        {
            foreach (var jugadorEleccion in JugadorImagenElegida)
            {
                string nombreJugador = jugadorEleccion.Key;
                List<string> imagenesSeleccionadas = jugadorEleccion.Value;

                var jugador = estadisticasPartida.Jugadores.SingleOrDefault(j => j.Nombre == nombreJugador);

                if (jugador != null && jugador.Nombre != NarradorActual)
                {
                    int puntosPorConfundir = 0;

                    foreach (var imagenId in imagenesSeleccionadas)
                    {
                        int votosRecibidos = JugadorImagenElegida
                            .Where(j => j.Key != nombreJugador)
                            .Count(j => j.Value.Contains(imagenId));

                        puntosPorConfundir += Math.Min(PUNTOS_MAXIMOS_RECIBIDOS_CONFUNDIR - puntosPorConfundir, votosRecibidos);

                        if (puntosPorConfundir >= PUNTOS_MAXIMOS_RECIBIDOS_CONFUNDIR)
                        {
                            break;
                        }
                    }

                    jugador.Puntos += puntosPorConfundir;
                }
            }
        }

        private async Task EjecutarRondaAsync()
        {
            RestablecerDesicionesJugadores();
            await EscogerNarradorAsync();
            AvisarQuienEsNarrador();

            await EsperarConfirmacionNarradorAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_NARRADOR));
            //TODO: El narrador no escogio nada
            //await El narrador no escogio nada
            if (SelecionoCartaNarrador)
            {

                // Espera por la confirmación de los jugadores de la imagen que escogen segun la pista
                await EsperarConfirmacionJugadoresAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_SELECCION));
                //Ir a la pantalla para mostrar todas las cartas elegidas
                MostrarGrupoCartas();
                CambiarPantalla(PANTALLA_TODOS_CARTAS , NarradorActual);
                await EsperarConfirmacionAdivinarAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_ADIVINAR));

            }
            else
            {
                //El narrador no escogio nada
            }



        }

        private void MostrarGrupoCartas()
        {
            string[] archivosCache = ObtenerArchivosCache();
            var archivoRutaMap = archivosCache.ToDictionary(ruta => Path.GetFileNameWithoutExtension(ruta), ruta => ruta);
            List<string> rutasCompletas = new List<string>();
            foreach (var listaDeNombres in JugadorImagenPuesta.Values)
            {
                foreach (var nombreArchivo in listaDeNombres)
                {
                    if (archivoRutaMap.TryGetValue(nombreArchivo, out var rutaCompleta))
                    {
                        rutasCompletas.Add(rutaCompleta);
                    }
                }
            }
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                jugadoresCallback.TryGetValue(nombre.ToString(), out IPartidaCallback callback);
                foreach (var rutaImagen in rutasCompletas)
                {

                    try
                    {
                        lectorDiscoOrquetador?.AsignarTrabajo(rutaImagen, callback, true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private async Task EsperarConfirmacionAdivinarAsync(TimeSpan tiempoEspera)
        {
            var tiempoParaJugadores = new CancellationTokenSource();
            var tareaEsperaJugadores = Task.Delay(tiempoEspera, tiempoParaJugadores.Token);

            while (!tareaEsperaJugadores.IsCompleted)
            {
                if (tareaEsperaJugadores.IsCompleted)
                {
                    Console.WriteLine("Tiempo agotado para los jugadores");
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            }

        }

        private void CambiarPantalla(int numeroPantalla)
        {
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                try
                {
                    jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback);
                    callback.CambiarPantallaCallback(numeroPantalla);
                }
                catch (Exception)
                {

                }
            };
        }
        /// <summary>
        /// Cambia la pantalla para todos los jugadores excluyendo a uno
        /// </summary>
        /// <param name="numeroPantalla">El número de la pantalla a mostrar</param>
        /// <param name="nombreExcluir">Indica si se debe excluir al narrador del cambio de pantalla</param>
        private void CambiarPantalla(int numeroPantalla, string nombreExcluir)
        {
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                if (nombreExcluir.Equals(nombre ,StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    if (jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback))
                    {
                        callback.CambiarPantallaCallback(numeroPantalla);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private async void AvisarQuienEsNarrador()
        {
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                try
                {
                    jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback);
                    if (!NarradorActual.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        callback?.NotificarNarradorCallback(false);
                    }
                }
                catch (Exception)
                {
                    await RemoverJugadorAsync(nombre);
                }
            };
        }

        private async Task EsperarConfirmacionNarradorAsync(TimeSpan tiempoEspera)
        {
            var tiempoParaNarrador = new CancellationTokenSource();
            var tareaEsperaNarrador = Task.Delay(tiempoEspera, tiempoParaNarrador.Token);

            while (NarradorActual == null || (ClaveImagenCorrectaActual == null && PistaActual == null))
            {
                if (tareaEsperaNarrador.IsCompleted)
                {
                    Console.WriteLine("Tiempo agotado para el narrador");
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            }
            if (ClaveImagenCorrectaActual == null || PistaActual == null || NarradorActual == null)
            {
                //TODO: Penalizar narrador
                SelecionoCartaNarrador = false;

                Console.Write($"Partida {IdPartida} el narrador {NarradorActual} no escogio nada se le penalizara");
                return;

            }
            SelecionoCartaNarrador = true;

        }

        private async Task EsperarConfirmacionJugadoresAsync(TimeSpan tiempoEspera)
        {
            var tiempoParaJugadores = new CancellationTokenSource();
            var tareaEsperaJugadores = Task.Delay(tiempoEspera, tiempoParaJugadores.Token);

            while (JugadoresPendientes.Any())
            {
                if (tareaEsperaJugadores.IsCompleted)
                {
                    Console.WriteLine("Tiempo agotado para los jugadores");
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            }

            if (!tareaEsperaJugadores.IsCompleted)
            {
                tiempoParaJugadores.Cancel();
                PenalizarJugadoresSinConfirmar();
            }
        }

        private void PenalizarJugadoresSinConfirmar()
        {
            foreach (var jugador in JugadoresPendientes)
            {
                if (jugadoresCallback.TryGetValue(jugador, out var callback))
                {
                    callback.TurnoPerdidoCallback();
                }
            }
        }

        private void RestablecerDesicionesJugadores()
        {
            NarradorActual = null;
            ClaveImagenCorrectaActual = null;
            PistaActual = null;
            SelecionoCartaNarrador = false;
            lock (JugadoresPendientes)
            {
                JugadoresPendientes = new ConcurrentBag<string>(jugadoresCallback.Keys);

            }
            lock (JugadorImagenElegida)
            {
                JugadorImagenElegida.Clear();

            }
            lock (JugadorImagenPuesta)
            {
                JugadorImagenPuesta.Clear();
            }
            JugadorImagenPuesta = new ConcurrentDictionary<string, List<string>>();
        }

        private async Task EscogerNarradorAsync()
        {
            var narrador = ObtenerNombresJugadores().OrderBy(x => random.Next()).FirstOrDefault();
            if (narrador == null)
            {
                return;
            }
            await semaphoreEscogerNarrador.WaitAsync();
            try
            {

                lock (jugadoresCallback)
                {
                    jugadoresCallback.TryGetValue(narrador, out IPartidaCallback callbackNarrador);
                    callbackNarrador.NotificarNarradorCallback(true);
                    NarradorActual = narrador;
                    Console.WriteLine("Se escogio a " + narrador + " como narrador");
                }
            }
            catch (Exception)
            {
                DesconectarUsuario(narrador);
            }
            finally
            {
                semaphoreEscogerNarrador.Release();
            }
            if (NarradorActual == null)
            {
                await EscogerNarradorAsync();
            }

        }



        public void ConfirmarTurnoAdivinarJugador(string nombreJugador, string claveImagen)
        {
            try
            {
                lock (JugadoresPendientes)
                {
                    JugadoresPendientes.TryTake(out nombreJugador);
                }
                JugadorImagenElegida.AddOrUpdate(
                    nombreJugador,
                    new List<string> { claveImagen },
                    (llave, listaExistente) =>         
                    {
                        if (!listaExistente.Contains(claveImagen))
                        {
                            listaExistente.Add(claveImagen);
                        }
                        return listaExistente;
                    }
                );
               
            }
            catch (Exception)
            {

            }
        }

        public void ConfirmacionTurnoEleccionJugador(string nombreJugador, string claveImagen)
        {
            try
            {
                lock (JugadoresPendientes)
                {
                    JugadoresPendientes.TryTake(out nombreJugador);
                }
                JugadorImagenPuesta.AddOrUpdate(
                    nombreJugador,
                    new List<string> { claveImagen },
                    (llave, listaExistente) =>            
                    {
                        if (!listaExistente.Contains(claveImagen))
                        {
                            listaExistente.Add(claveImagen);
                        }
                        return listaExistente;
                    }
                );
            }
            catch (Exception)
            {

            }
        }

        internal void ConfirmarTurnoNarrador(string nombreJugador, string claveImagen, string pista)
        {
            this.ClaveImagenCorrectaActual = claveImagen;
            this.PistaActual = pista;
            try
            {
                lock (JugadoresPendientes)
                {
                    JugadoresPendientes.TryTake(out nombreJugador);
                }
                JugadorImagenElegida.AddOrUpdate(
                    nombreJugador,
                    new List<string> { claveImagen },
                    (key, existingList) => 
                    {
                        if (!existingList.Contains(claveImagen))
                        {
                            existingList.Add(claveImagen);
                        }
                        return existingList;
                    }
                );
                JugadorImagenPuesta.AddOrUpdate(
                   nombreJugador,
                   new List<string> { claveImagen },
                   (llave, listaExistente) =>
                   {
                       if (!listaExistente.Contains(claveImagen))
                       {
                           listaExistente.Add(claveImagen);
                       }
                       return listaExistente;
                   }
               );
            }
            catch (Exception)
            {
            }
            MostrarPistaJugadores();

        }

        private async Task TerminarPartidaAsync()
        //FIXME: Este metodo termina la partida independientemente de lo que este pasando
        {
            //Cacular los puntos de rondas menores a 3 no hacer nada
            await CalcularPuntos();

            //Avisar a todos de la terminacion
            AvisarPartidaTerminada();
            //Eliminar la partida (despues de esto la partida ya no existe)
            EliminarPartida();
        }

        private async Task CalcularPuntos()
        {
            if (RondaActual > RONDAS_MINIMA_PARA_PUNTOS)
            {
                var listaNoInvitado = jugadoresInformacion.Values
                    .Where(jugador => jugador.idUsuario > ID_INVALIDO)
                    .Select(jugador =>
                        {
                            try
                            {
                                int idEstadistica = estadisticasDAO.ObtenerIdEstadisticaConIdUsuario(jugador.idUsuario);
                                return idEstadistica != 0 ? new Tuple<string, int>(jugador.gamertag, idEstadistica) : null;
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        })
                    .Where(tuple => tuple != null)
                    .ToList();

                if (listaNoInvitado.Any())
                {
                    await estadisticasPartida.GuardarPuntajeAsync(listaNoInvitado);
                }
            }
        }

        private void AvisarPartidaTerminada()
        {
            lock (jugadoresCallback)
            {
                foreach (var nombre in jugadoresCallback)
                {
                    jugadoresCallback.TryGetValue(nombre.ToString(), out IPartidaCallback callback);
                    callback?.FinalizarPartida();
                }
            }
        }

        private bool VerificarCondicionVictoria()
        {
            return condicionVictoria.Verificar(this);
        }

        private void MostrarPistaJugadores()
        {
            lock (jugadoresCallback)
            {
                foreach (var nombre in ObtenerNombresJugadores())
                {
                    //TODO: tal vez aqui no deberiamos llamar al narrador pero lo podemos ocupar 
                    //para indicar que el narrador ya escogio por lo tanto se pasa a la siguiente fase de 
                    //la ronda;
                    jugadoresCallback.TryGetValue(nombre.ToString(), out IPartidaCallback callback);
                    callback.MostrarPistaCallback(this.PistaActual);
                }
            }

        }

        #endregion Ronda
    }
}
