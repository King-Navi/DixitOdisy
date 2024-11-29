using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuarioSesion_Prueba : ConfiguradorPruebaParaServicio
    {

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
        }

        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        #region ObtenerSessionJugadorCallback
        [TestMethod]
        public void ObtenerSessionJugadorCallback_SeAbreElCanal_DeberiaRetornarTrue()
        {
            
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            
            manejador.ObtenerSesionJugador(usuario);

            
            Assert.IsTrue(implementacionCallback.SesionAbierta, "El callback debería haber sido llamado y la sesión debería estar activa.");
            Assert.AreEqual(CommunicationState.Opened, ((ICommunicationObject)implementacionCallback).State, "El canal debería estar en estado abierto.");
            Assert.IsTrue(manejador.YaIniciadoSesion(usuario.Nombre), "El canal debería estar en estado abierto.");

            implementacionCallback.Close();

        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_SeCierraElCanal_DeberiaRetornarTrue()
        {
            
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            
            manejador.ObtenerSesionJugador(usuario);

            
            Assert.IsTrue(implementacionCallback.SesionAbierta, "El callback debería haber sido llamado y la sesión debería estar activa.");
            Assert.AreEqual(CommunicationState.Opened, ((ICommunicationObject)implementacionCallback).State, "El canal debería estar en estado abierto.");
            Assert.IsTrue(manejador.YaIniciadoSesion(usuario.Nombre), "El canal debería estar en estado abierto.");

            implementacionCallback.Close();
            var resultado = manejador.YaIniciadoSesion("PruebaUsuario");
            Assert.IsFalse(resultado, "No deberia estar inciado");

        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_YaHaIniciadoSesion_DeberiaRetornarUsuarioFalla()
        {
            

            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            var usuarioRepetido = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            : 
            manejador.ObtenerSesionJugador(usuario);

            //Assert
            var excepcion = Assert.ThrowsException<FaultException<UsuarioFalla>>(() => manejador.ObtenerSesionJugador(usuarioRepetido));
            Assert.IsTrue(excepcion.Detail.EstaConectado, "La excepción debería indicar que el usuario ya está conectado.");
            implementacionCallback.Close();
        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_EnDesconeccion_DeberiaRetornarFalse()
        {
            

            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            : Llamar a ObtenerSessionJugador una vez para establecer la sesión
            manejador.ObtenerSesionJugador(usuario);
            implementacionCallback.Abort();

            //Assert
            Assert.IsFalse(manejador.YaIniciadoSesion(usuario.Nombre), "No deberia estar iniciado.");
            if (implementacionCallback != null)
            {
                implementacionCallback.Abort();
            }

        }
        [TestMethod]
        public void ObtenerSessionJugadorCallback_EsNulo_DeberiaRetornarFalse()
        {
            

            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            Usuario usuario = null;

            : Llamar a ObtenerSessionJugador una vez para establecer la sesión
            manejador.ObtenerSesionJugador(usuario);

            //Assert
            Assert.IsFalse(implementacionCallback.SesionAbierta, "El callback no debería haber sido llamado");
            if (implementacionCallback != null)
            {
                implementacionCallback.Abort();
            }

        }
        #endregion ObtenerSessionJugadorCallback


    }

}

