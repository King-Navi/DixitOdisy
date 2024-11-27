using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;
using WcfServicioLibreria.Enumerador;
using DAOLibreria;
using System.IO;

namespace Pruebas.Servidor
{
    /// <summary>
    /// Prueba de <see cref="WcfServicioLibreria.Contratos.IServicioPartidaSesion" />
    /// que a su vez llama a  <see cref="IServicioPartidaSesion"/>
    /// </summary>
    [TestClass]
    public class ServicioPartidaSesion
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;
        private ConfiguracionPartida configuracionGenerica;

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
            configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 0);

        }
        #region UnirsePartida
        [TestMethod]
        public async Task UnirsePartida_PartidaValida_DeberiaInvocarCallbackAvisoNuevoJugador()
        {
            // Arrange
            //Precondicion el Usuario deberia estar en BD
            var implementacionCallback = new PartidaCallbackImpl();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IPartidaCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
       
            // Act
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Assert
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Any(jugador => jugador.Key == usuario.Nombre), "El callback debería haber sido llamado y la sesión debería estar activa.");

        }
        [TestMethod]
        public async Task UnirsePartida_PartidaNoValida_NoDeberiaAgregarJugadorNiInvocarCallback()
        {
            // Arrange
            string gamertag = "JugadorInvalido";
            string idPartidaInexistente = "partidaNoExistente";

            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            // Act
            await manejador.UnirsePartidaAsync(gamertag, idPartidaInexistente);

            // Assert
            Assert.IsFalse(implementacionCallback.JugadoresEnPartida.Any(j => j.Key == gamertag), "El jugador no debería haberse agregado ya que la partida no es válida.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaVacia_NoDeberiaExistirPartidaRetornaFalse()
        {
            // Arrange
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };

            // Act
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Assert
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
            Assert.IsFalse(manejador.ValidarPartida(idPartida), "No deberia existir la partida");
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaConJugadorIgual_RetornaTrue()
        {
            // Arrange
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 19, Nombre = "navi" };

            // Act
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);

            // Assert
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Count < 2, "El jugador debería haberse agregado (total jugadores 2).");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaConJugador_DeberiaExistir2Jugadores()
        {
            // Arrange
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 1, Nombre = "NaviKing" };

            // Act
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);

            // Assert
            Console.WriteLine(implementacionCallback.JugadoresEnPartida.Count);
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Count == 2, "El jugador debería haberse agregado (total jugadores 2).");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task UnirsePartida_JugadorInvitado_InvitadoConImagen()
        {
            // Arrange
            //Precondicion: el invitado no existe en BD
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var invitado = new Usuario { IdUsuario = -1, Nombre = "GUESTNoExisteEnBD" };

            // Act
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(invitado.Nombre, idPartida);

            // Assert
            Console.WriteLine(implementacionCallback.JugadoresEnPartida.Count);
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Count == 2, "Debería haberse agregado (total jugadores 2).");
            implementacionCallback.JugadoresEnPartida.TryGetValue(invitado.Nombre, out Usuario invitadoRecibido);
            Assert.IsTrue(invitadoRecibido.FotoUsuario is MemoryStream, "El invitado debería haberse agregado con MemoryStream");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        #endregion
        #region SolicitarImagen
        [TestMethod]
        public async Task SolicitarImagenCarta_PartidaValida_DeberiaEnviarImagen()
        {
            // Arrange
            //PRECAUCION: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP 
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Act: Llamar a SolicitarImagenCarta
            await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            await Task.Delay(TimeSpan.FromSeconds(10));
            // Assert: Verificar que el callback RecibirImagenCallback fue llamado con una imagen válida
            Assert.IsNotNull(implementacionCallback.UltimaImagenRecibida, "El jugador debería haber recibido una imagen.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarVariasImagenes_DeberiaEnviarImagenes()
        {
            // Arrange
            //PRECAUCION: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP 
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Act: Llamar a SolicitarImagenCarta
            await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Assert: Verificar que el callback RecibirImagenCallback fue llamado con una imagen válida
            Assert.IsNotNull(implementacionCallback.UltimaImagenRecibida, "El jugador debería haber recibido una imagen.");
            Assert.IsTrue(implementacionCallback.Imagenes.Count == 3, "El jugador debería haber recibido una imagen.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarMuchasImagenes_DeberiaEnviarImagenes()
        {
            // Arrange
            //PRECAUCION: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP 
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Act: Llamar a SolicitarImagenCarta
            // Lista de tareas para realizar las solicitudes en paralelo
            var tareasSolicitudes = new List<Task>();
            var numeroSolicitudes = 12;
            for (int i = 0; i < numeroSolicitudes; i++)
            {
                tareasSolicitudes.Add(manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida));
            }

            // Espera a que todas las tareas se completen
            await Task.WhenAll(tareasSolicitudes);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Assert: Verificar que el callback RecibirImagenCallback fue llamado con una imagen válida
            Assert.IsNotNull(implementacionCallback.UltimaImagenRecibida, "El jugador debería haber recibido una imagen.");
            Assert.IsTrue(implementacionCallback.Imagenes.Count == numeroSolicitudes, "El jugador debería haber recibido una imagen.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarImagenHastaDIOS_DeberiaEnviarImagenes()
        {
            // Arrange
            //PRECAUCION: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP y escribir en disco
            //PRECAUCION: Este metodo gasta credito (DINERO REAL), solo para mostrar a profe descomentar la linea de abajo
            string rutaCarpeta = null;
            //rutaCarpeta = Path.Combine("..", "..", "..", "WcfServicioLibreria", "Recursos", "Mitologia");
            if (!Directory.Exists(rutaCarpeta))
            {
                Console.WriteLine("Ruta completa: " + Path.GetFullPath(rutaCarpeta));
                Assert.Fail("El directorio de la WCFLibreria no se encontro");
            }
            string[] archivosJpg = Directory.GetFiles(rutaCarpeta, "*.jpg");
            Console.WriteLine("Imagenes en disco: " + archivosJpg.Length);

            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mitologia, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            // Act: Llamar a SolicitarImagenCarta hasta que aparesca dios
            foreach (var archivo in archivosJpg)
            {
                await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            }
            await manejador.SolicitarImagenCartaAsync(usuario.Nombre, idPartida);
            await Task.Delay(TimeSpan.FromSeconds(15));

            // Assert: Verificar que el callback RecibirImagenCallback fue llamado con una imagen válida

            Assert.IsNotNull(implementacionCallback.UltimaImagenRecibida, "El jugador debería haber recibido una imagen.");
            Assert.IsTrue(implementacionCallback.Imagenes.Count == archivosJpg.Length +1, "El jugador debería haber recibido una imagen.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }

        #endregion
        #region EmpezarPartida
        [TestMethod]
        public async Task EmpezarPartida_NoHaySuficienteJugadores_EliminaPartida()
        {
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
            //Precondicion: El usuario debe existir
            //Precondicon:  El Tiempo de espera de conexiones debe ser igual a la de la partida
            int EsperaJugadores = 22;
            var anfitrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var idPartida = manejador.CrearPartida(anfitrion.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(anfitrion.Nombre, idPartida);
            await manejador.EmpezarPartidaAsync(anfitrion.Nombre, idPartida);
            await Task.Delay(TimeSpan.FromSeconds(EsperaJugadores));
            Assert.IsFalse(manejador.ValidarPartida(idPartida), "La partida no deberia exisitir");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        
        [TestMethod]
        public async Task EmpezarPartida_SuficienteJugadores_PartidaEnProgreso()
        {
            Assert.Fail(); //TODO: Terminar la preuba
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
            //Precondicion: El usuario debe existir
            var anfitrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var idPartida = manejador.CrearPartida(anfitrion.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(anfitrion.Nombre, idPartida);
            await manejador.EmpezarPartidaAsync(anfitrion.Nombre, idPartida);
            Assert.IsFalse(manejador.ValidarPartida(idPartida), "La partida no deberia exisitir");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task EmpezarPartida_IdNull_RetornaVoid()
        {
            var implementacionCallback = new PartidaCallbackImpl();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
            //Precondicion: El usuario debe existir
            var anfitrion = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var idPartida = manejador.CrearPartida(anfitrion.Nombre, configuracionGenerica);

            await manejador.UnirsePartidaAsync(anfitrion.Nombre, idPartida);
            await manejador.EmpezarPartidaAsync(anfitrion.Nombre,null);
            Assert.IsTrue(manejador.ValidarPartida(idPartida), "La partida deberia exisitir");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }

        #endregion EmpezarPartida
    }
}
