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
        private const int CANTIDAD_MINIMA_JUGADORES = 0; // 3
        private const int TIEMPO_ESPERA_JUGADORES = 10;// 20
        private const int TIEMPO_ESPERA_NARRADOR = 20; // 40
        private const int TIEMPO_ESPERA_SELECCION = 20; //60
        private const int TIEMPO_ESPERA_PARA_CONFIRMAR = 5; //5

        private const int JUGADORES_PARTIDA_VACIA = 0; //0
        private const int NADIE_ACERTO = 0;//0
        private const int RONDAS_MINIMA_PARA_PUNTOS = 3; //3
        private const int PUNTOS_RESTADOS_NO_PARTICIPAR = 1; //1
        private const int PUNTOS_ACIERTO = 1; //1
        private const int AUMENTO_POR_PENALIZACION_NARRADOR = 2; //2
        private ConcurrentDictionary<string, IPartidaCallback> jugadoresCallback = new ConcurrentDictionary<string, IPartidaCallback>();
        private readonly ConcurrentDictionary<string, DesconectorEventoManejador> eventosCommunication = new ConcurrentDictionary<string, DesconectorEventoManejador>();
        private ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario> jugadoresInformacion = new ConcurrentDictionary<string, DAOLibreria.ModeloBD.Usuario>();
        private readonly ICondicionVictoria condicionVictoria;
        public ConfiguracionPartida Configuracion { get; private set; }
        private EstadisticasPartida estadisticasPartida;

        private string rutaImagenes;
        private static readonly Random random = new Random();
        private static readonly SemaphoreSlim semaphoreDisco = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim semaphoreEscogerNarrador = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEmpezarPartida = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreLeerFotoInvitado = new SemaphoreSlim(1, 1);
        private static readonly HttpClient httpCliente = new HttpClient();

        public EventHandler partidaVaciaManejadorEvento;

        private event EventHandler todosListos;

        private string[] archivosCache;

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
        public bool PartidaEnProgreso { get; private set; } = false;

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
            estadisticasPartida = new EstadisticasPartida(_configuracion.Tematica);

            todosListos += (sender, e) => Task.Run(async () => await IniciarPartidaSeguroAsync());
            archivosCache = Directory.GetFiles(rutaImagenes, "*.jpg");

        }


        #endregion Contructor

        #region Metodos

        #region Imagenes
        public async Task EnviarImagen(string nombreSolicitante) 
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
                    resultado = await SolicitarImagenChatGPTAsync(); 
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

        private async Task<ImagenCarta> LeerImagenDiscoAsync() //FIXME: Cambiar a hilos como el escritor
        {
            if (archivosCache == null || archivosCache.Length == 0)
            {
                archivosCache = Directory.GetFiles(rutaImagenes, "*.jpg");
            }

            var archivosRestantes = archivosCache.Where(a => !ImagenesUsadas.Contains(Path.GetFileNameWithoutExtension(a))).ToArray();

            if (archivosRestantes.Length == 0)
            {
                return await SolicitarImagenChatGPTAsync();
            }

            await semaphoreDisco.WaitAsync();
            try
            {
                string archivoAleatorio = archivosRestantes[random.Next(archivosRestantes.Length)];
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoAleatorio);
                byte[] imagenBytes = await Task.Run(() => File.ReadAllBytes(archivoAleatorio));
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
                Escritor.EncolarEscritura(memoryStream, rutaDestino);
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

            EnPartidaVacia();
        }



        #endregion ManejarEstadoPartida

        #region InicializarPartida
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


        #endregion InicializarPartida

        #region Ronda

        public async Task EmpezarPartida()
        {
            await semaphoreEmpezarPartida.WaitAsync();
            try
            {
                if (PartidaEnProgreso)
                {
                    return;
                }

                DateTime inicioEspera = DateTime.Now;

                while ((DateTime.Now - inicioEspera).TotalSeconds < TIEMPO_ESPERA_JUGADORES)
                {
                    await Task.Delay(1000);
                }

                if (!PartidaEnProgreso && jugadoresCallback.Count >= CANTIDAD_MINIMA_JUGADORES)
                {
                    PartidaEnProgreso = true;
                    todosListos?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    //TODO: No empezar ya que no se cumplio el minimo en un tiempo determinado
                    TerminarPartida();
                }
            }
            finally
            {
                semaphoreEmpezarPartida.Release();
            }
        }

        private async Task IniciarPartidaSeguroAsync()
        {
            try
            {
                await EjecutarRondasAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar ronda {RondaActual} :  {ex.Message} ");
            }
        }
        
        private async Task EjecutarRondasAsync()
        {
            while (!VerificarCondicionVictoria() && ContarJugadores() >= CANTIDAD_MINIMA_JUGADORES)
            {
                //Tiempos de ronda
                Console.WriteLine("Ronda: " + RondaActual);
                await EjecutarRondaAsync();
                ///Evaluar puntajes de ronda y manda a la pantalla
                await EvaluarPuntosRondaAsync();
                //Regresa a pantalla 1
                CambiarPantalla(1);
                ++RondaActual;
            }

            TerminarPartida();
        }

        private async Task EvaluarPuntosRondaAsync()  //FIXME
        {
            //TODO: Necesitaremos una clase que nos ayude aqui y sea un data contract para estarla pasando

            CalculoPuntosEnSituaciones();

            //TODO:Mostrar resultados
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
            CambiarPantalla(4);
            await Task.Delay(5000);

        }

        private void CalculoPuntosEnSituaciones()
        {
            bool alguienAdivinoImagen = false;
            bool todosAdivinaron = true;
            int totalJugadores = estadisticasPartida.Jugadores.Count;
            int votosCorrectos = 0;

            // Contar cuántos jugadores acertaron la imagen correcta
            foreach (var jugadorEleccion in JugadorImagenElegida)
            {
                var jugador = estadisticasPartida.Jugadores.SingleOrDefault(j => j.Nombre == jugadorEleccion.Item1);

                if (jugador != null)
                {
                    if (jugadorEleccion.Item2 == ClaveImagenCorrectaActual)
                    {
                        jugador.Puntos += PUNTOS_ACIERTO;
                        votosCorrectos++;
                        alguienAdivinoImagen = true;
                    }
                    else
                    {
                        todosAdivinaron = false; 
                    }
                }
                else
                {
                    //PUEDE SER NULO
                    //SI LA CONDICION DE ARRIBA EVALUA QUE NO SEA NULO ESTE ES EL CASO EN EL QUE ES NULO
                    // Jugador que no participó, resta 1 punto
                    //jugador.Puntos -= PUNTOS_RESTADOS_NO_PARTICIPAR;
                }
            }

            // Evaluar condiciones de puntuación    todos acertaron o nadie acerto
            if (votosCorrectos == NADIE_ACERTO || votosCorrectos == totalJugadores)
            {
                foreach (var jugador in estadisticasPartida.Jugadores)
                {
                    // El cuentacuentos no recibe puntos ya que es el que esta siendo penalizado
                    if (jugador.Nombre != NarradorActual) 
                    {
                        jugador.Puntos += AUMENTO_POR_PENALIZACION_NARRADOR;
                    }
                }
            }
            else
            {
                // Si hay jugadores que acertaron (ya se asignaron los 3 puntos arriba)
                // asignamos 1 punto a los jugadores por cada voto recibido, máximo 3
                foreach (var jugadorEleccion in JugadorImagenElegida)
                {
                    var jugador = estadisticasPartida.Jugadores.SingleOrDefault(j => j.Nombre == jugadorEleccion.Item2);
                    if (jugador != null && jugador.Nombre != NarradorActual)
                    {
                        jugador.Puntos += Math.Min(3, JugadorImagenElegida.Count(j => j.Item2 == jugador.Nombre));
                    }
                }
            };
        }

        private async Task EjecutarRondaAsync()
        {
            await EscogerNarradorAsync();
            AvisarQuienEsNarrador();

            // Espera por la confirmación del narrador
            await EsperarConfirmacionNarradorAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_NARRADOR));
            //TODO: El narrador no escogio nada
            //await El narrador no escogio nada

            // Espera por la confirmación de los jugadores
            await EsperarConfirmacionJugadoresAsync(TimeSpan.FromSeconds(TIEMPO_ESPERA_SELECCION));

        }

        /// <summary>
        ///  1.- Inicio ronda
        /// Escoger carta jugador = 2
        /// EscogerCataNarrador = 3
        ///  4.- Estadisitcas
        ///  5.- Fin Partida
        /// </summary>
        /// <param name="numeroPantalla"></param>
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

        private void AvisarQuienEsNarrador()
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
                    RemoverJugador(nombre);
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

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_CONFIRMAR));
            }
            if (ClaveImagenCorrectaActual == null || PistaActual == null || NarradorActual == null)
            {
                throw new Exception("No faltan valores");
            }
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

                await Task.Delay(TimeSpan.FromSeconds(TIEMPO_ESPERA_PARA_CONFIRMAR));
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
            lock (JugadoresPendientes)
            {
                JugadoresPendientes = new ConcurrentBag<string>(jugadoresCallback.Keys);

            }
            lock (JugadorImagenElegida)
            {
                JugadorImagenElegida = new ConcurrentBag<Tuple<string, string>>();

            }
        }

        private async Task EscogerNarradorAsync()
        {
            RestablecerDesicionesJugadores();
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
                    //No evaluar si es nulll, ya que puede ser null y lo tendria que sacar
                    callbackNarrador.NotificarNarradorCallback(true);
                    NarradorActual = narrador;
                    Console.WriteLine("Se escogio a " + narrador +" como narrador");
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
            if (NarradorActual == null) // Si falló, intenta de nuevo fuera del bloqueo del semáforo
            {
                await EscogerNarradorAsync();
            }

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

        internal void ConfirmacionTurnoNarrador(string nombreJugador, string claveImagen, string pista)
        {
            this.ClaveImagenCorrectaActual = claveImagen;
            this.PistaActual = pista;
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
            MostrarPistaJugadores();

        }

        private void TerminarPartida()
        //FIXME: Este metodo termina la partida independientemente de lo que este pasando
        {
            //Cacular los puntos de rondas menores a 3 no hacer nada
            CalcularPuntos();

            //Avisar a todos de la terminacion
            AvisarPartidaTerminada();
            //Eliminar la partida (despues de esto la partida ya no existe)
            EliminarPartida();
        }

        private void CalcularPuntos()
        {
            //TODO: Calcular puntos solo si fueron mas de 3 rondas;
            if (RondaActual > RONDAS_MINIMA_PARA_PUNTOS)
            {

            }
            else
            {

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

        #region ManejoJugadores
        internal bool AgregarJugador(string nombreJugador, IPartidaCallback nuevoContexto) //FIXME
        {
            //TODO: Si partida progreso tal vez deberiamos evitar que se unan 
            bool resultado = false;
            jugadoresCallback.AddOrUpdate(nombreJugador, nuevoContexto, (key, oldValue) => nuevoContexto);
            //TODO:Asegurarse que es un contexto valido, tal vez no es necesario
            //try
            //{
            //    nuevoContexto.Ping();
            //}
            //catch (Exception)
            //{
            //    jugadoresCallback.TryRemove(nombreJugador, out _);
            //    return false;
            //}
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
                    string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "FotosInvitados");
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
            if (eventosJugador != null)
            {
                eventosJugador.Desechar();
            }
            if (ContarJugadores() == JUGADORES_PARTIDA_VACIA)
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

        #endregion ManejoJugadores


        #endregion Metodos
    }



}

