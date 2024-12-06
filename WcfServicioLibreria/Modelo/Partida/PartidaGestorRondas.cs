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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                    if (jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback))
                    {
                        Task _ = Task.Run(() =>
                        {
                            try
                            {
                                callback.EnviarEstadisticasCallback(estadisticasPartida);
                            }
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                            }
                        });
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            CambiarPantalla(PANTALLA_ESTADISTICAS);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_MOSTRAR_ESTADISTICAS));

        }

        private void CalculoPuntosEnSituaciones()
        {
            if (SelecionoCartaNarrador)
            {
                IPuntaje puntaje = new Puntaje(NarradorActual, 
                    estadisticasPartida.Jugadores, 
                    ImagenElegidaPorJugador, 
                    ImagenesTodosGrupo,
                    ClaveImagenCorrectaActual);
                puntaje.CalcularPuntaje();
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
                    jugador.Puntos -= Puntaje.PUNTOS_RESTADOS_NO_PARTICIPAR;
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
                await EsperarEleccionesJugadoresAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_SELECCION));
                await EnMostrarTodasCartas();
                CambiarPantalla(PANTALLA_TODOS_CARTAS , NarradorActual);
                await EsperarConfirmacionAdivinarAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_ADIVINAR));
            }
            else
            {
                EnviarMensajePerdidaTurno(NarradorActual);
            }
        }

        private void EnviarMensajePerdidaTurno(string narradorActual)
        {
            if (String.IsNullOrEmpty(narradorActual))
            {
                return;
            };
            try
            {
                jugadoresCallback.TryGetValue(narradorActual, out IPartidaCallback callback);
                if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                {
                    callback.TurnoPerdidoCallback();
                }
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }

        }

        public async Task EnMostrarTodasCartas()
        {
            var todasLasImagenesGrupo = ImagenesTodosGrupo.Values.SelectMany(lista => lista).ToList();
            RondaEventArgs evento = new RondaEventArgs(todasLasImagenesGrupo);
            MostrarTodasLasCartas?.Invoke(this, evento);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ENVIO_SEGUNDOS));
        }

        private async Task EsperarConfirmacionAdivinarAsync(TimeSpan tiempoEspera)
        {
            var cancelacion = new CancellationTokenSource();
            var tareaEsperaJugadores = Task.Delay(tiempoEspera, cancelacion.Token);

            while (!tareaEsperaJugadores.IsCompleted)
            {
                if (tareaEsperaJugadores.IsCompleted)
                {
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
                    callback.CambiarPantallaCallback(numeroPantalla);
                }
                catch (TimeoutException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                }
                catch (Exception excepcion)
                {
                    await RemoverJugadorAsync(nombre);
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            }
            if (ClaveImagenCorrectaActual == null || PistaActual == null || NarradorActual == null)
            {
                SelecionoCartaNarrador = false;
                return;
            }
            SelecionoCartaNarrador = true;
        }

        private async Task EsperarEleccionesJugadoresAsync(TimeSpan tiempoEspera)
        {
            var cancelacion = new CancellationTokenSource();
            var tareaEsperaJugadores = Task.Delay(tiempoEspera, cancelacion.Token);

            while (JugadoresPendientes.Any())
            {
                if (tareaEsperaJugadores.IsCompleted)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            }

            if (!tareaEsperaJugadores.IsCompleted)
            {
                cancelacion.Cancel();
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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
            ImagenesTodosGrupo.Clear();
            ImagenesTodosGrupo = new ConcurrentDictionary<string, List<string>>();
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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                DesconectarUsuario(narrador);
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                Console.WriteLine($" ConfirmarTurnoAdivinarJugador   Me llamo  {nombreJugador}");
                JugadoresPendientes.TryTake(out nombreJugador);
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
                Console.WriteLine($"Me llamo y lo agregue a las lista de eleciones {nombreJugador}");
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public void ConfirmacionTurnoEleccionJugador(string nombreJugador, string claveImagen)
        {
            try
            {
                Console.WriteLine($"ConfirmacionTurnoEleccionJugador Me llamo para su imagen {nombreJugador}");
                //JugadoresPendientes.TryTake(out nombreJugador);
                ImagenesTodosGrupo.AddOrUpdate(
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
                Console.WriteLine($"Me llamo y lo agregie a las lista de todos {nombreJugador}");

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        internal void ConfirmarTurnoNarrador(string nombreJugador, string claveImagen, string pista)
        {
            this.ClaveImagenCorrectaActual = claveImagen;
            this.PistaActual = pista;
            try
            {
                Console.WriteLine($"Me llamo el narrador {nombreJugador}");
                JugadoresPendientes.TryTake(out nombreJugador );
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
                ImagenesTodosGrupo.AddOrUpdate(
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
                Console.WriteLine($"Me llamo el narrador {nombreJugador} y lo agregie a las listas");
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                                return idEstadistica != ID_INVALIDO_ESTADISTICAS ? new Tuple<string, int>(jugador.gamertag, idEstadistica) : null;
                            }
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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

                    Task.Run(() =>
                    {
                        try
                        {
                            callback?.MostrarPistaCallback(this.PistaActual);
                        }
                        catch (TimeoutException excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                        catch (CommunicationException excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                        catch (Exception excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                    });
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }
    }
    #endregion Ronda
}

