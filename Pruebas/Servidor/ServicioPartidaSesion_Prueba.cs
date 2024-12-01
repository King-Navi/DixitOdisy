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
using Pruebas.Servidor.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioPartidaSesion_Prueba : ConfiguradorPruebaParaServicio
    {
        private ConfiguracionPartida configuracionGenerica;

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }
        #region UnirsePartida
        [TestMethod]
        public async Task UnirsePartida_PartidaValida_DeberiaInvocarCallbackAvisoNuevoJugador()
        {
            
            var implementacionCallback = new PartidaCallbackImplementacion();

            mockContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IPartidaCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
       
            
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Any(jugador => jugador.Key == usuario.Nombre), "El callback debería haber sido llamado y la sesión debería estar activa.");

        }
        [TestMethod]
        public async Task UnirsePartida_PartidaNoValida_NoDeberiaAgregarJugadorNiInvocarCallback()
        {
            
            string gamertag = "JugadorInvalido";
            string idPartidaInexistente = "partidaNoExistente";

            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            
            await manejador.UnirsePartidaAsync(gamertag, idPartidaInexistente);

            
            Assert.IsFalse(implementacionCallback.JugadoresEnPartida.Any(j => j.Key == gamertag), "El jugador no debería haberse agregado ya que la partida no es válida.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaVacia_NoDeberiaExistirPartidaRetornaFalse()
        {
            
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };

            
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);

            
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
            Assert.IsFalse(manejador.ValidarPartida(idPartida), "No deberia existir la partida");
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaConJugadorIgual_RetornaTrue()
        {
            
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 19, Nombre = "navi" };

            
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);

            
            Assert.IsTrue(implementacionCallback.JugadoresEnPartida.Count < 2, "El jugador debería haberse agregado (total jugadores 2).");
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task UnirsePartida_PartidaConJugador_DeberiaExistir2Jugadores()
        {
            
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var usuarioNuevo = new Usuario { IdUsuario = 1, Nombre = "NaviKing" };

            
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(usuarioNuevo.Nombre, idPartida);

            
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
            
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);

            var usuarioExistente = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var invitado = new Usuario { IdUsuario = -1, Nombre = "GUESTNoExisteEnBD" };

            
            var idPartida = manejador.CrearPartida(usuarioExistente.Nombre, configuracionGenerica);
            await manejador.UnirsePartidaAsync(usuarioExistente.Nombre, idPartida);
            await manejador.UnirsePartidaAsync(invitado.Nombre, idPartida);

            
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
     
        #region EmpezarPartida
        [TestMethod]
        public async Task EmpezarPartida_NoHaySuficienteJugadores_EliminaPartida()
        {
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
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
            Assert.Fail(); 
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
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
            var implementacionCallback = new PartidaCallbackImplementacion();
            mockContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionCallback);
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
