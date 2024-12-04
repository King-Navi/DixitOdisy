using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo.Evento;
using WcfServicioLibreria.Utilidades;

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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
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
            catch (OperationCanceledException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            await TerminarPartidaAsync();
        }

        private async Task EvaluarPuntosRondaAsync()
        {
            CalculoPuntosEnSituaciones();
            estadisticasPartida.CalcularPodio();
            try
            {
                foreach (var nombre in ObtenerNombresJugadores().ToList())
                {
                    jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback);
                    callback.EnviarEstadisticasCallback(estadisticasPartida);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            CambiarPantalla(PANTALLA_ESTADISTICAS);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_MOSTRAR_ESTADISTICAS));

        }

        private void CalculoPuntosEnSituaciones()
        {
            if (SelecionoCartaNarrador)
            {
                VerificarAciertos(out bool alguienAdivinoImagen, out bool todosAdivinaron, out int votosCorrectos);
                AplicarPenalizacionNoParticipacion();
                EvaluarCondicionesGlobales(votosCorrectos, todosAdivinaron);
                AsignarPuntosPorConfundir();
            }
            else
            {
                AplicarPenlizacionNarrador();
            }
        }

        private void AplicarPenlizacionNarrador()
        {
            foreach (var jugador in estadisticasPartida.Jugadores)
            {
                if (jugador.Nombre.Equals(NarradorActual, StringComparison.OrdinalIgnoreCase))
                {
                    jugador.Puntos -= PUNTOS_RESTADOS_NO_PARTICIPAR;
                }
            }
        }

        private void VerificarAciertos(out bool alguienAdivinoImagen, out bool todosAdivinaron, out int votosCorrectos)
        {
            alguienAdivinoImagen = false;
            todosAdivinaron = true;
            votosCorrectos = 0;

            foreach (var jugadorEleccion in ImagenElegidaPorJugador)
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
                    && ImagenElegidaPorJugador.TryGetValue(jugador.Nombre, out var imagenesSeleccionadas)
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
            foreach (var jugadorEleccion in ImagenElegidaPorJugador)
            {
                string nombreJugador = jugadorEleccion.Key;
                List<string> imagenesSeleccionadas = jugadorEleccion.Value;

                var jugador = estadisticasPartida.Jugadores.SingleOrDefault(j => j.Nombre == nombreJugador);

                if (jugador != null && jugador.Nombre != NarradorActual)
                {
                    int puntosPorConfundir = 0;

                    foreach (var imagenId in imagenesSeleccionadas)
                    {
                        int votosRecibidos = ImagenElegidaPorJugador
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
            await AvisarQuienEsNarradorAsync();
            await EsperarConfirmacionNarradorAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_NARRADOR));
            if (SelecionoCartaNarrador)
            {
                await EsperarConfirmacionJugadoresAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_SELECCION));
                MostrarGrupoCartas();
                CambiarPantalla(PANTALLA_TODOS_CARTAS , NarradorActual);
                await EsperarConfirmacionAdivinarAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_ADIVINAR));
            }
            else
            {
                EnviarMensajePenalizacion(NarradorActual);
            }
        }

        private void EnviarMensajePenalizacion(string narradorActual)
        {
            if (String.IsNullOrEmpty(narradorActual))
            {
                return;
            };
            try
            {
                jugadoresCallback.TryGetValue(narradorActual, out IPartidaCallback callback);
                if (true)
                {
                    
                }
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

        }

        public async Task EnMostrarTodasCartas()
        {
            var todasLasImagenes = ImagenPuestasPisina.Values.SelectMany(lista => lista).ToList();
            RondaEventArgs evento = new RondaEventArgs(todasLasImagenes);
            MostrarTodasLasCartas?.Invoke(this, evento);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ENVIO_SEGUNDOS));
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
                    if (((ICommunicationObject)callback).State != CommunicationState.Opened)
                    {
                        continue;
                    }
                    callback.TurnoPerdidoCallback();
                }
                catch (TimeoutException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
            };
        }

        private void CambiarPantalla(int numeroPantalla, string nombreExcluir)
        {
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                if (nombreExcluir.Equals(nombre, StringComparison.OrdinalIgnoreCase))
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
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
            }
        }

        private async Task AvisarQuienEsNarradorAsync()
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
                catch (TimeoutException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (Exception excepcion)
                {
                    await RemoverJugadorAsync(nombre);
                    ManejadorExcepciones.ManejarErrorException(excepcion);
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
            try
            {
                foreach (var jugador in JugadoresPendientes)
                {
                    if (jugadoresCallback.TryGetValue(jugador, out var callback))
                    {
                        callback.TurnoPerdidoCallback();
                    }
                }
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }

        private void RestablecerDesicionesJugadores()
        {
            NarradorActual = null;
            ClaveImagenCorrectaActual = null;
            PistaActual = null;
            SelecionoCartaNarrador = false;
            JugadoresPendientes = new ConcurrentBag<string>(jugadoresCallback.Keys);
            ImagenElegidaPorJugador.Clear();
            ImagenPuestasPisina.Clear();
            ImagenPuestasPisina = new ConcurrentDictionary<string, List<string>>();
        }

        private async Task EscogerNarradorAsync()
        {
            var narrador = ObtenerNombresJugadores().OrderBy(x => aleatorio.Next()).FirstOrDefault();
            if (narrador == null)
            {
                return;
            }
            await semaphoreEscogerNarrador.WaitAsync();
            try
            {
                jugadoresCallback.TryGetValue(narrador, out IPartidaCallback callbackNarrador);
                callbackNarrador.NotificarNarradorCallback(true);
                NarradorActual = narrador;
                Console.WriteLine("Se escogio a " + narrador + " como narrador");
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                DesconectarUsuario(narrador);
                ManejadorExcepciones.ManejarErrorException(excepcion);
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
                ImagenElegidaPorJugador.AddOrUpdate(
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
                ImagenPuestasPisina.AddOrUpdate(
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
                ImagenElegidaPorJugador.AddOrUpdate(
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
                ImagenPuestasPisina.AddOrUpdate(
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
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            MostrarPistaJugadores();

        }

        private async Task TerminarPartidaAsync()
        {
            await EnviarResultadoBaseDatos();
            AvisarPartidaTerminada();
            EliminarPartida();
        }

        private async Task EnviarResultadoBaseDatos()
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
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarErrorException(excepcion);
                            }
                            return null;
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
            try
            {
                foreach (var nombre in jugadoresCallback)
                {
                    jugadoresCallback.TryGetValue(nombre.ToString(), out IPartidaCallback callback);
                    if (((ICommunicationObject)callback).State != CommunicationState.Opened)
                    {
                        continue;
                    }
                    callback?.FinalizarPartidaCallback();
                }
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }

        private bool VerificarCondicionVictoria()
        {
            return condicionVictoria.Verificar(this);
        }

        private void MostrarPistaJugadores()
        {
            try
            {
                foreach (var nombre in ObtenerNombresJugadores())
                {
                    jugadoresCallback.TryGetValue(nombre.ToString(), out IPartidaCallback callback);
                    callback?.MostrarPistaCallback(this.PistaActual);
                }
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }
    }
    #endregion Ronda
}

