using System.Collections.Concurrent;
using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Enumerador;
using System.ServiceModel;
using WcfServicioLibreria.Utilidades;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ChatGPTLibreria;
using ChatGPTLibreria.ModelosJSON;
using System.Net.Http;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://refactoring.guru/es/design-patterns/strategy</ref>
    internal class Partida : IObservador//FIXME: Faltan muchas funcionalidades y pruebas
    {
        #region Atributos
        private ConcurrentDictionary<string, IPartidaCallback> jugadoresCallback = new ConcurrentDictionary<string, IPartidaCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private readonly ICondicionVictoria condicionVictoria;
        public ConfiguracionPartida Configuracion { get; private set; }
        public EventHandler partidaVaciaManejadorEvento;
        private string rutaImagenes;
        private bool partidaEmpezada = false;
        private static readonly object imagenLock = new object();
        private static readonly Random random = new Random();
        private static readonly SemaphoreSlim semaphoreDisco = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreChatGPT = new SemaphoreSlim(1, 1);
        private static readonly HttpClient httpCliente = new HttpClient();

        #endregion Atributos

        #region Propiedad
        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public string Tematica { get; private set; }
        public int RondaActual { get; private set; }
        public int CartasRestantes { get; private set; }
        private ConcurrentBag<string> ImagenesUsadas { get; set; }
        public ConcurrentBag<string> JugadoresPendientes { get; private set; }
        private ConcurrentBag<Tuple<string, string>> JugadorImagenElegida { get; set; }
        private IEscribirDisco Escritor { get; set; }
        private SolicitarImagen SolicitarImagen { get; set; }
        #endregion Propiedad

        #region Contructor
        public Partida(string _idPartida, string _anfitrion, ConfiguracionPartida _configuracion, IEscribirDisco _escritorDisco)
        {
            IdPartida = _idPartida;
            Anfitrion = _anfitrion;
            condicionVictoria = CrearCondicionVictoria(_configuracion);
            rutaImagenes = CalcularRutaImagenes(_configuracion);
            Tematica = _configuracion.Tematica.ToString();
            CartasRestantes = Directory.GetFiles(rutaImagenes, "*.jpg").Length;
            ImagenesUsadas = new ConcurrentBag<string>();
            JugadoresPendientes = new ConcurrentBag<string>();
            JugadorImagenElegida = new ConcurrentBag<Tuple<string, string>>();
            RondaActual = 0;
            Escritor = _escritorDisco;
            SolicitarImagen = new SolicitarImagen();

        }


        # endregion Contructor
        #region Metodos
        #region Imagenes
        public async Task EnviarImagen(string nombreSolicitante) //FIXME
        {
            ImagenCarta imagenCarta = await CalcularNuevaImagen();

            try
            {
                jugadoresCallback.TryGetValue(nombreSolicitante, out IPartidaCallback callback);

                callback?.RecibirImagenCallback(imagenCarta);

            }
            catch (Exception)
            {

            }
        }

        private async Task<ImagenCarta> CalcularNuevaImagen()
        {
            ImagenCarta resultado = null;
            try
            {
                if (CartasRestantes <= 0)
                {
                    resultado = await SolicitarImagenChatGPTAsync(); //FIXME
                }
                else
                {
                    resultado = await LeerImagenDiscoAsync();
                    CartasRestantes--;
                }
            }
            catch (Exception)
            {

            }
            return resultado;
        }

        private async Task<ImagenCarta> LeerImagenDiscoAsync()
        {
            await semaphoreDisco.WaitAsync();
            try
            {
                var archivos = Directory.GetFiles(rutaImagenes, "*.jpg");

                if (archivos.Length == 0)
                {
                    return await SolicitarImagenChatGPTAsync();
                }

                var archivosRestantes = archivos.Where(a => !ImagenesUsadas.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();

                if (archivosRestantes.Length == 0)
                {
                    return await SolicitarImagenChatGPTAsync();
                }
                string archivoAleatorio = archivosRestantes[random.Next(archivosRestantes.Length)];
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
                byte[] imagenBytes = File.ReadAllBytes(archivoAleatorio);
                ImagenesUsadas.Add(nombreSinExtension);
                return new ImagenCarta
                {
                    IdImagen = nombreSinExtension,
                    ImagenStream = new MemoryStream(imagenBytes)
                };
            }
            finally
            {
                semaphoreDisco.Release();
            }
        }

        private async Task<ImagenCarta> SolicitarImagenChatGPTAsync()
        {

            ImagenCarta resultado = null;
            ImagenPedido64JSON imagenPedido = new ImagenPedido64JSON("Genera una imagen basada en la tematica " + Tematica + " debe ser rectangular y vertical");

            var respuesta = await SolicitarImagen.EjecutarImagenPrompt64JSON(imagenPedido, httpCliente);

            if (respuesta?.ImagenDatosList != null && respuesta.ImagenDatosList.Any())
            {
                var imagenBytes = Convert.FromBase64String(respuesta.ImagenDatosList[0].Base64Imagen);
                MemoryStream memoryStream = new MemoryStream(imagenBytes);
                string rutaDestino = Path.Combine(rutaImagenes, $"{Guid.NewGuid()}.jpg");
                Escritor.EncolarEscrituraAsync(memoryStream, rutaDestino);
                resultado = new ImagenCarta
                {
                    ImagenStream = memoryStream,
                    IdImagen = Path.GetFileNameWithoutExtension(rutaDestino)
                };
            }
            return resultado;
        }

        private string CalcularRutaImagenes(ConfiguracionPartida configuracion)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos");
            switch (configuracion.Tematica)
            {
                case TematicaPartida.Mixta:
                    return Path.Combine(ruta, "Mixta");
                case TematicaPartida.Mitologia:
                    return Path.Combine(ruta, "Mitologia");
                case TematicaPartida.Animales:
                    return Path.Combine(ruta, "Animales");
                case TematicaPartida.Paises:
                    return Path.Combine(ruta, "Paises");
                default:
                    return Path.Combine(ruta, "Mixta");
            }
        }
        #endregion Imagenes
        private void EnPartidaVacia()
        {
            partidaVaciaManejadorEvento?.Invoke(this, new PartidaVaciaEventArgs(DateTime.Now, this));
        }

        private ICondicionVictoria CrearCondicionVictoria(ConfiguracionPartida condicionVictoria)
        {
            switch (condicionVictoria.Condicion)
            {
                case CondicionVictoriaPartida.PorCantidadRondas:
                    if (condicionVictoria.NumeroRondas < 6)
                    {
                        return new CondicionVictoriaPorRondas(6);
                    }
                    else
                    {
                        return new CondicionVictoriaPorRondas(condicionVictoria.NumeroRondas);
                    }
                case CondicionVictoriaPartida.PorCartasAgotadas:
                    return new CondicionVictoriaCartasAgotadas();
                default:
                    return new CondicionVictoriaCartasAgotadas();
            }
        }

        public void EmpezarPartida(string nombre)
        {
            lock (JugadoresPendientes)
            {
                if (nombre != null)
                {
                    JugadoresPendientes.TryTake(out nombre);
                }
                if (!partidaEmpezada && JugadoresPendientes.Count < 0)
                {
                    partidaEmpezada = true;
                    CambiarRonda();

                }
            }

        }

        private async void CambiarRonda() //FIXME
        {
            //LimpiarConcurrentbags
            lock (JugadoresPendientes)
            {
                JugadoresPendientes = new ConcurrentBag<string>(jugadoresCallback.Keys);

            }
            lock (JugadorImagenElegida)
            {
                JugadorImagenElegida = new ConcurrentBag<Tuple<string, string>>();

            }
            //Escoger Narrador
            await EscogerNarrador();
            //
            if (VerificarCondicionVictoria())
            {
                TerminarPartida();
                return;
            }
            else
            {
                //TODO:Seguir juando
                var tiempoParaSiguienteRonda = new CancellationTokenSource();
                // Espera por confirmación de los jugadores
                var tareaEspera = Task.Delay(TimeSpan.FromSeconds(60), tiempoParaSiguienteRonda.Token);
                while (JugadoresPendientes.Any())
                {

                    // Si el tiempo se cumple rompe el bucle
                    if (tareaEspera.IsCompleted)
                    {
                        break;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }

                // Si termina el tiempo penaliza a los que no confirmaron
                if (!tareaEspera.IsCompleted)
                {
                    tiempoParaSiguienteRonda.Cancel();
                    foreach (var jugador in JugadoresPendientes)
                    {
                        if (jugadoresCallback.TryGetValue(jugador, out var callback))
                        {
                            //callback.TurnoPerdido();
                        }
                    }
                }
                ++RondaActual;
                CambiarRonda();

            }
        }

        private async Task EscogerNarrador() //FIXME
        {
        }

        public void ConfirmacionTurnoJugador(string nombreJugador, string claveImagen)
        {
            try
            {
                lock (JugadoresPendientes)
                {
                    JugadoresPendientes.TryTake(out nombreJugador);
                }
                lock (JugadorImagenElegida)
                {
                    JugadorImagenElegida.Add(new Tuple<string, string>(nombreJugador, claveImagen));
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void TerminarPartida() //FIXME
        {
            throw new NotImplementedException();
        }

        private bool VerificarCondicionVictoria()
        {
            return condicionVictoria.Verificar(this);
        }

        internal bool AgregarJugador(string nombreJugador, IPartidaCallback nuevoContexto) //FIXME
        {
            bool resultado = false;
            jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
            if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback contextoCambiado))
            {
                if (ReferenceEquals(nuevoContexto, contextoCambiado))
                {
                    eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                    resultado = true;
                    lock (JugadoresPendientes)
                    {
                        JugadoresPendientes.Add(nombreJugador);
                    }
                }
            }
            return resultado;
        }

        internal void AvisarNuevoJugador(string nombreJugador)
        {
            DAOLibreria.ModeloBD.Usuario informacionUsuario = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            lock (jugadoresCallback)
            {
                lock (jugadoresInformacion)
                {
                    jugadoresInformacion.TryAdd(nombreJugador, informacionUsuario);
                    Usuario usuario = new Usuario
                    {
                        Nombre = informacionUsuario.gamertag,
                        FotoUsuario = new MemoryStream(informacionUsuario.fotoPerfil)
                        //TODO: Si se necesita algo mas del jugador nuevo colocar aqui
                    };
                    // enviar la lista completa de jugadores al nuevo jugador
                    if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback nuevoCallback))
                    {
                        foreach (var jugadorExistente in jugadoresInformacion.Values)
                        {
                            Usuario jugador = new Usuario
                            {
                                Nombre = jugadorExistente.gamertag,
                                FotoUsuario = new MemoryStream(jugadorExistente.fotoPerfil)
                            };
                            nuevoCallback.ObtenerJugadorPartidaCallback(jugador);
                        }
                    }
                    // enviar la información del nuevo jugador a los jugadores existentes
                    foreach (var jugadorConectado in ObtenerNombresJugadores())
                    {
                        if (jugadoresCallback.TryGetValue(jugadorConectado, out IPartidaCallback callback))
                        {
                            if (jugadorConectado != nombreJugador)
                            {
                                callback.ObtenerJugadorPartidaCallback(usuario);
                            }
                        }
                    }

                }
            }
        }

        private void AvisarRetiroJugador(string nombreUsuarioEliminado)
        {
            lock (jugadoresCallback)
            {
                lock (jugadoresInformacion)
                {
                    jugadoresInformacion.TryRemove(nombreUsuarioEliminado, out DAOLibreria.ModeloBD.Usuario usuarioEliminado);
                    if (usuarioEliminado != null)
                    {
                        Usuario usuario = new Usuario
                        {
                            Nombre = usuarioEliminado.gamertag
                        };
                        foreach (var nombreJugador in ObtenerNombresJugadores())
                        {
                            jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback callback);
                            if (callback != null)
                            {
                                try
                                {
                                    callback.EliminarJugadorPartidaCallback(usuario);

                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }


                }
            }
        }

        public void DesconectarUsuario(string nombreJugador)
        {
            AvisarRetiroJugador(nombreJugador);
            RemoverJugador(nombreJugador);
        }

        private void RemoverJugador(string nombreJugador)
        {
            jugadoresCallback.TryRemove(nombreJugador, out IPartidaCallback _);
            eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
            jugadoresInformacion.TryRemove(nombreJugador, out _);
            if (eventosJugador != null)
            {
                eventosJugador.Desechar();
            }
            if (ContarJugadores() == 0)
            {
                EliminarPartida();
            }
        }

        private void EliminarPartida()
        {
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadores();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadores();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresCallback.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresCallback[clave]).Close();
                }
            }
            jugadoresCallback.Clear();
            EnPartidaVacia();
        }

        private IReadOnlyCollection<string> ObtenerNombresJugadores()
        {
            return jugadoresCallback.Keys.ToList().AsReadOnly();
        }

        private int ContarJugadores()
        {
            return jugadoresCallback.Count;
        }
        #endregion
    }



}

