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
using System.ServiceModel.Security;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://refactoring.guru/es/design-patterns/strategy</ref>
    internal partial class Partida : IObservador//FIXME: Faltan muchas funcionalidades y pruebas
    {
        #region Constantes
        #region Carpetas
        private const string RUTA_RECURSOS = "Recursos";
        private const string CARPETA_MIXTA = "Mixta";
        private const string CARPETA_MITOLOGIA = "Mitologia";
        private const string CARPETA_ANIMALES = "Animales";
        private const string CARPETA_PAISES = "Paises";
        private const string CARPETA_FOTOS_INVITADOS = "FotosInvitados";
        #endregion Carpetas
        #region PantallasCliente
        private const int PANTALLA_INICIO = 1;
        private const int PANTALLA_NARRADOR_SELECION = 2;
        private const int PANTALLA_JUGADOR_SELECION = 3;
        private const int PANTALLA_TODOS_CARTAS = 4;
        private const int PANTALLA_ESTADISTICAS = 5;
        private const int PANTALLA_FIN_PARTIDA = 6;
        private const int PANTALLA_ESPERA = 7;
        #endregion PantallasCliente
        private const int CANTIDAD_MINIMA_JUGADORES = 0; // 2
        private const int TIEMPO_ESPERA_UNIRSE_JUGADORES = 20;// 20
        private const int TIEMPO_ESPERA_NARRADOR = 40; // 40
        private const int TIEMPO_ESPERA_SELECCION = 10; //60
        private const int TIEMPO_ESPERA_PARA_ADIVINAR = 20; //60
        private const int TIEMPO_ESPERA = 5; //5
        private const int NUM_JUGADOR_PARTIDA_VACIA = 0; //0
        private const int NUM_JUGADOR_NADIE_ACERTO = 0;//0
        private const int RONDAS_MINIMA_PARA_PUNTOS = 3; //3
        private const int NUMERO_MINIMO_RONDAS = 6; //6
        private const int LIMITE_CARTAS_MINIMO = 0; //0
        private const int NO_ARCHIVOS_RESTANTES = 0; //0
        private const int PUNTOS_RESTADOS_NO_PARTICIPAR = 1; //1
        private const int PUNTOS_ACIERTO = 1; //1
        private const int PUNTOS_PENALIZACION_NARRADOR = 2; //2
        private const int PUNTOS_MAXIMOS_RECIBIDOS_CONFUNDIR = 3; //3
        private const int TIEMPO_MOSTRAR_ESTADISTICAS = 10; //10
        private const int ID_INVALIDO = 0;
        #endregion Constantes
        #region Atributos


        private readonly ConcurrentDictionary<string, IPartidaCallback> jugadoresCallback = new ConcurrentDictionary<string, IPartidaCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private readonly ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private readonly ICondicionVictoria condicionVictoria;
        public ConfiguracionPartida Configuracion { get; private set; }
        private readonly EstadisticasPartida estadisticasPartida;

        private readonly string rutaImagenes;
        private static readonly Random random = new Random();
        private static readonly SemaphoreSlim semaphoreEscogerNarrador = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEmpezarPartida = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEnviandoImagenDeChatGPT = new SemaphoreSlim(1, 1);
        private readonly object lockCartasRestantes = new object();
        private static readonly HttpClient httpCliente = new HttpClient();
        public EventHandler partidaVaciaManejadorEvento;
        private event EventHandler TodosListos;
        private readonly Lazy<string[]> archivosCache;
        private readonly LectorDisco lectorDisco = new LectorDisco();
        #endregion Atributos

        #region Propiedad
        public string IdPartida { get; private set; }
        public string Anfitrion { get; private set; }
        public string Tematica { get; private set; }
        public string NarradorActual { get; private set; }
        public string ClaveImagenCorrectaActual { get; private set; }
        public string PistaActual { get; private set; }
        public int RondaActual { get; private set; }
        public int CartasRestantes { get; private set; }
        public bool SeLlamoEmpezarPartida { get; private set; } = false;
        public bool SeTerminoEsperaUnirse { get; private set; } = false;
        public bool SelecionoCartaNarrador { get; private set; } = false;
        /// <summary>
        /// Imagenes ya ocupadas
        /// </summary>
        private ConcurrentBag<string> ImagenesUsadas { get; set; }
        /// <summary>
        /// Jugadores que aun no han confirmado su selecion
        /// </summary>
        public ConcurrentBag<string> JugadoresPendientes { get; private set; }
        /// <summary>
        /// Diccionario para la piscina de cartas
        /// </summary>
        private ConcurrentDictionary<string, List<string>> JugadorImagenPuesta { get; set; } = new ConcurrentDictionary<string, List<string>>();
        /// <summary>
        /// Diccionario para la eleccion de los jugadores
        /// </summary>
        private ConcurrentDictionary<string, List<string>> JugadorImagenElegida { get; set; }
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
            JugadorImagenElegida = new ConcurrentDictionary<string, List<string>>();
            RondaActual = 0;
            Escritor = _escritorDisco;
            SolicitarImagen = new SolicitarImagen();
            estadisticasPartida = new EstadisticasPartida(_configuracion.Tematica);

            TodosListos += (sender, e) => Task.Run(async () => await IniciarPartidaSeguroAsync());
            archivosCache = new Lazy<string[]>(() => Directory.GetFiles(rutaImagenes, "*.jpg"));

        }


        #endregion Contructor

        #region Metodos

        #region Imagenes
        public async Task<bool> EnviarImagen(string nombreSolicitante) 
        {
            Console.WriteLine($"Método EnviarImagen llamado.");
            return await CalcularNuevaImagen(nombreSolicitante);

        }

        private async Task<bool> CalcularNuevaImagen(string nombreSolicitante)
        {
            bool resultado = false;
            try
            {
                if (CartasRestantes <= LIMITE_CARTAS_MINIMO)
                {
                    resultado = await SolicitarImagenChatGPTAsync(nombreSolicitante); 
                }
                else
                {
                    resultado = await LeerImagenDiscoAsync(nombreSolicitante);
                    lock (lockCartasRestantes)
                    {
                        CartasRestantes--;
                    }
                }
            }
            catch (Exception)
            {
                Console.Write("Error en el metodo CalcularNuevaImagen(string nombreSolicitante)");
            }
            return true;
        }

        private string[] ObtenerArchivosCache()
        {
            return archivosCache.Value;
        }

        private async Task<bool> LeerImagenDiscoAsync(string nombreSolicitante)
        {
            string[] archivosCache = ObtenerArchivosCache();

            var archivosRestantes = archivosCache.Where(a => !ImagenesUsadas.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();

            if (archivosRestantes.Length == NO_ARCHIVOS_RESTANTES)
            {
                Console.WriteLine($"No quedan imágenes disponibles. Solicitud externa requerida para petion de {nombreSolicitante}");
                return await SolicitarImagenChatGPTAsync(nombreSolicitante);
            }
            string archivoAleatorio = archivosRestantes[random.Next(archivosRestantes.Length)];
            string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
            try
            {
                jugadoresCallback.TryGetValue(nombreSolicitante, out IPartidaCallback callback);
                lectorDisco.EncolarLecturaEnvio(archivoAleatorio, callback);
                ImagenesUsadas.Add(nombreSinExtension);
                return true;
            }
            catch (Exception)
            {
            }
            return false;

        }

        private async Task<bool> SolicitarImagenChatGPTAsync(string nombreSolicitante)
        {

            ImagenPedido64JSON imagenPedido = new ImagenPedido64JSON("Genera una imagen basada en la tematica " + Tematica + " debe ser rectangular y vertical");

            var respuesta = await SolicitarImagen.EjecutarImagenPrompt64JSON(imagenPedido, httpCliente);

            if (respuesta?.ImagenDatosList != null && respuesta.ImagenDatosList.Any())
            {
                var imagenBytes = Convert.FromBase64String(respuesta.ImagenDatosList[0].Base64Imagen);
                MemoryStream memoryStream = new MemoryStream(imagenBytes);
                string rutaDestino = Path.Combine(rutaImagenes, $"{Guid.NewGuid()}.jpg");
                Escritor.EncolarEscritura(memoryStream, rutaDestino);
                ImagenCarta resultado = new ImagenCarta
                {
                    ImagenStream = memoryStream,
                    IdImagen = Path.GetFileNameWithoutExtension(rutaDestino)
                };
                await semaphoreEnviandoImagenDeChatGPT.WaitAsync();
                try
                {
                    jugadoresCallback.TryGetValue(nombreSolicitante, out IPartidaCallback callback);

                    callback?.RecibirImagenCallback(resultado);
                    return true;
                }
                catch (Exception)
                {
                }
                finally
                {
                    semaphoreEnviandoImagenDeChatGPT.Release();

                }
            }
            return false;
        }

        private string CalcularRutaImagenes(ConfiguracionPartida configuracion)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RUTA_RECURSOS);
            switch (configuracion.Tematica)
            {
                case TematicaPartida.Mixta:
                    return Path.Combine(ruta, CARPETA_MIXTA);
                case TematicaPartida.Mitologia:
                    return Path.Combine(ruta, CARPETA_MITOLOGIA);
                case TematicaPartida.Animales:
                    return Path.Combine(ruta, CARPETA_ANIMALES);
                case TematicaPartida.Paises:
                    return Path.Combine(ruta, CARPETA_PAISES);
                default:
                    return Path.Combine(ruta, CARPETA_MIXTA);
            }
        }
        #endregion Imagenes

        #region ManejarEstadoPartida
        private void EnPartidaVacia()
        {
            partidaVaciaManejadorEvento?.Invoke(this, new PartidaVaciaEventArgs(DateTime.Now, this));
        }

        private void EliminarPartida()
        {
            IReadOnlyCollection<string> claveJugadores = ObtenerNombresJugadores();
            foreach (var clave in claveJugadores)
            {
                if (jugadoresCallback.ContainsKey(clave))
                {
                    ((ICommunicationObject)jugadoresCallback[clave]).Close();
                }
            }
            jugadoresCallback.Clear();
            IReadOnlyCollection<string> claveEventos = ObtenerNombresJugadores();
            foreach (var clave in claveEventos)
            {
                if (eventosCommunication.ContainsKey(clave))
                {
                    eventosCommunication[clave].Desechar();
                }
            }
            eventosCommunication.Clear();
            lectorDisco.DetenerLectura();
            EnPartidaVacia();
        }

        #endregion ManejarEstadoPartida

        #region InicializarPartida
        private ICondicionVictoria CrearCondicionVictoria(ConfiguracionPartida condicionVictoria)
        {
            switch (condicionVictoria.Condicion)
            {
                case CondicionVictoriaPartida.PorCantidadRondas:
                    if (condicionVictoria.NumeroRondas < NUMERO_MINIMO_RONDAS)
                    {
                        return new CondicionVictoriaPorRondas(NUMERO_MINIMO_RONDAS);
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


        #endregion InicializarPartida

        #region ManejoJugadores
        internal bool AgregarJugador(string nombreJugador, IPartidaCallback nuevoContexto) //FIXME
        {
            //TODO: Si partida progreso tal vez deberiamos evitar que se unan 
            bool resultado = false;
            jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
            if (jugadoresCallback.TryGetValue(nombreJugador, out IPartidaCallback contextoCambiado))
            {
                if (ReferenceEquals(nuevoContexto, contextoCambiado))
                {
                    eventosCommunication.TryAdd(nombreJugador, new DesconectorEventoManejador((ICommunicationObject)contextoCambiado, this, nombreJugador));
                    //// Verificar si todos los jugadores están listos
                    //if (TodosEstanListos())
                    //{
                    //    // Disparar el evento
                    //    TodosListos?.Invoke(this, EventArgs.Empty);
                    //}
                    resultado = true;
                }
            }
            return resultado;
        }

        internal async Task AvisarNuevoJugadorAsync(string nombreJugador)
        {
            DAOLibreria.ModeloBD.Usuario informacionUsuario = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            bool esInvitado = false;
            if (informacionUsuario == null)
            {
                await semaphoreLeerFotoInvitado.WaitAsync();
                try
                {
                    //TODO: Hacer algo si no hay imagenes
                    string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RUTA_RECURSOS, CARPETA_FOTOS_INVITADOS);
                    var archivos = Directory.GetFiles(ruta, "*.png");
                    string archivoAleatorio = archivos[random.Next(archivos.Length)];
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
                    esInvitado = true;
                    informacionUsuario = new DAOLibreria.ModeloBD.Usuario
                    {
                        gamertag =nombreJugador, 
                        fotoPerfil = File.ReadAllBytes(archivoAleatorio)
                    };
                }
                catch (Exception)
                {
                }
                finally
                {
                    semaphoreLeerFotoInvitado.Release();
                }
            }
            lock (jugadoresCallback)
            {
                lock (jugadoresInformacion)
                {
                    jugadoresInformacion.TryAdd(nombreJugador, informacionUsuario);
                    Usuario usuario = new Usuario
                    {
                        Nombre = informacionUsuario.gamertag,
                        FotoUsuario = new MemoryStream(informacionUsuario.fotoPerfil),
                        EsInvitado = esInvitado
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
            //TODO:Revisar si se va alguien importante en un momento critico (Podemos ocupar estado para ciertos puntos de la partida)
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
            //TODO: EVALUAR QUE EL QUE SE DESCONECTA NO SEA EL NARRADOR O ALGO IMPORTANTE
            AvisarRetiroJugador(nombreJugador);
            RemoverJugador(nombreJugador);
        }

        private void RemoverJugador(string nombreJugador)
        {
            jugadoresCallback.TryRemove(nombreJugador, out IPartidaCallback _);
            eventosCommunication.TryRemove(nombreJugador, out DesconectorEventoManejador eventosJugador);
            jugadoresInformacion.TryRemove(nombreJugador, out _);
            eventosJugador?.Desechar();
            if (ContarJugadores() == NUM_JUGADOR_PARTIDA_VACIA)
            {
                EliminarPartida();
            }
        }

        private IReadOnlyCollection<string> ObtenerNombresJugadores()
        {
            return jugadoresCallback.Keys.ToList().AsReadOnly();
        }

        private int ContarJugadores()
        {
            return jugadoresCallback.Count;
        }

        internal void ConfirmarInclusionPartida(string gamertag, IPartidaCallback contexto)
        {
            try
            {
                contexto.IniciarValoresPartidaCallback(true);
            }
            catch (Exception)
            {

            };
        }

        #endregion ManejoJugadores


        #endregion Metodos
    }



}

