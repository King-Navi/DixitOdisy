using DAOLibreria.ModeloBD;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
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
                if (SeLlamoEmpezarPartidaPrimeraVez)
                {
                    return;
                }
                SeLlamoEmpezarPartidaPrimeraVez = true;
                ManejarInicioPartida();
            }
            finally
            {
                semaphoreEmpezarPartida.Release();
            }
        }

        private void ManejarInicioPartida()
        {
            try
            {
                Task.Run(async () =>
                {
                    DateTime inicioEspera = DateTime.Now;

                    while ((DateTime.Now - inicioEspera).TotalSeconds < TIEMPO_ESPERA_UNIRSE_JUGADORES)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
                    }

                    if (!SeLlamoEmpezarPartida && jugadoresCallback.Count >= CANTIDAD_MINIMA_JUGADORES)
                    {
                        SeLlamoEmpezarPartida = true;
                        TodosListos?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        NotifcarNoIncioPartida();
                        await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
                        await TerminarPartidaAsync();
                    }
                });
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }



        private void NotifcarNoIncioPartida()
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
                            callback?.NoSeEmpezoPartidaCallback();
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


        private async Task IniciarPartidaSeguroAsync()
        {
            try
            {
                estadisticasPartida.AgregarDesdeOtraLista(ObtenerNombresJugadores());
                await EjecutarRondasAsync();
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

        private async Task EjecutarRondasAsync()
        {
            using (var cancelacion = new CancellationTokenSource())
            {
                try
                {
                    if (DebeCancelarRondas)
                    {
                        cancelacion.Cancel();
                    }

                    while (!VerificarCondicionVictoria() && ContarJugadores() >= CANTIDAD_MINIMA_JUGADORES && !cancelacion.Token.IsCancellationRequested)
                    {
                        await EjecutarRondaAsync();
                        await EvaluarPuntosRondaAsync();
                        CambiarPantalla(PANTALLA_INICIO);
                        await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA), cancelacion.Token);
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
                finally
                {
                    await TerminarPartidaAsync();
                }
            }
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
                        bool esAnfitrion = false;
                        if (nombre.Equals(Anfitrion, StringComparison.OrdinalIgnoreCase))
                        {
                            esAnfitrion = true;
                        }
                        await Task.Run(() =>
                        {
                            try
                            {
                                callback.EnviarEstadisticasCallback(estadisticasPartida, esAnfitrion);
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
                    ImagenesTablero,
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
                await CambiarPantallaConExclusionAsync(PANTALLA_TODOS_CARTAS, NarradorActual);
                await MostrarTableroCartasAsync();
                await Task.Delay(TIEMPO_ESPERA);
                JugadoresPendientes = new ConcurrentDictionary<string, bool>(
                    jugadoresCallback.ToDictionary(
                    pair => pair.Key,
                    pair => true));
                await EsperarConfirmacionAdivinarAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_ADIVINAR));
            }
            else
            {
                EnviarMensajePerdidaTurno(NarradorActual);
            }
        }

        private async Task MostrarTableroCartasAsync()
        {
            try
            {
                foreach (var tuplaJugadorContexto in jugadoresCallback)
                {
                    if (tuplaJugadorContexto.Key.Equals(NarradorActual, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    await Task.Run(() =>
                    {
                        try
                        {
                            tuplaJugadorContexto.Value?.MostrarTableroCartasCallback();
                        }
                        catch (TimeoutException excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                        catch (CommunicationException excepcion)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excepcion);
                        }
                    });
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                Task.Run(() =>
                {
                    try
                    {
                        jugadoresCallback.TryGetValue(narradorActual, out IPartidaCallback callback);
                        if (callback == null)
                        {
                            return;
                        }
                        if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                        {
                            callback?.TurnoPerdidoCallback();
                        }
                    }
                    catch (TimeoutException)
                    {
                        throw;
                    }
                    catch (CommunicationException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });

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

        private async Task EsperarConfirmacionAdivinarAsync(TimeSpan tiempoEspera)
        {
            using (var cancelacion = new CancellationTokenSource())
            {
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
                    await EnviarMensajeJugadoresSinConfirmarAsync();
                }
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

        private async Task CambiarPantallaConExclusionAsync(int numeroPantalla, string nombreExcluir)
        {
            foreach (var nombre in ObtenerNombresJugadores().ToList())
            {
                if (nombreExcluir.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    await Task.Run(() =>
                    {
                        if (jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback))
                        {
                            callback?.CambiarPantallaCallback(numeroPantalla);
                        }
                    });

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
                await Task.Run(() =>
                {
                    try
                    {
                        jugadoresCallback.TryGetValue(nombre, out IPartidaCallback callback);
                        if (!NarradorActual.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                        {
                            callback?.NotificarNarradorCallback(false);
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
                });
            };
        }

        private async Task EsperarConfirmacionNarradorAsync(TimeSpan tiempoEspera)
        {
            using (var tiempoParaNarrador = new CancellationTokenSource())
            {
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
        }

        private async Task EsperarEleccionesJugadoresAsync(TimeSpan tiempoEspera)
        {
            using (var cancelacion = new CancellationTokenSource())
            {
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
                    await EnviarMensajeJugadoresSinConfirmarAsync();
                }
            }
        }

        private async Task EnviarMensajeJugadoresSinConfirmarAsync()
        {
            try
            {
                foreach (var jugador in JugadoresPendientes.Keys)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            if (jugadoresCallback.TryGetValue(jugador, out var callback))
                            {
                                if (callback == null)
                                {
                                    return;
                                }
                                if (((ICommunicationObject)callback).State == CommunicationState.Opened
                                    || ((ICommunicationObject)callback).State == CommunicationState.Opening)
                                {
                                    callback?.TurnoPerdidoCallback();
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
                    });
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
            JugadoresPendientes = new ConcurrentDictionary<string, bool>(
                jugadoresCallback.ToDictionary(
                pair => pair.Key,
                pair => true));
            ImagenElegidaPorJugador.Clear();
            ImagenesTablero.Clear();
            ImagenesTablero = new ConcurrentDictionary<string, List<string>>();
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
                await DesconectarUsuarioAsync(narrador);
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
                JugadoresPendientes.TryRemove(nombreJugador, out _);
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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public void ConfirmacionTurnoEleccionJugador(string nombreJugador, string claveImagen)
        {
            try
            {
                JugadoresPendientes.TryRemove(nombreJugador, out _);
                ImagenesTablero.AddOrUpdate(
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
                JugadoresPendientes.TryRemove(nombreJugador, out _);
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
                ImagenesTablero.AddOrUpdate(
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
            var copiaDiccionario = jugadoresInformacion.ToDictionary(
                entry => entry.Key,
                entry => entry.Value
            );
            CambiarPantalla(PANTALLA_FIN_PARTIDA);
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            await EnviarResultadoBaseDatos(copiaDiccionario);
            AvisarPartidaTerminada();
            await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA));
            EliminarPartida();
        }

        private async Task EnviarResultadoBaseDatos(Dictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion)
        {
            if (RondaActual >= RONDAS_MINIMA_PARA_PUNTOS)
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
                    Task.Run(() =>
                    {
                        try
                        {
                            callback?.FinalizarPartidaCallback();
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

