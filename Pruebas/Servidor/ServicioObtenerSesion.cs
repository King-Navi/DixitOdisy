using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuarioSesion
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }
        [TestMethod]
        public void ObtenerSessionJugadorCallback_SeAbreElCanal_DeberiaRetornarTrue()
        {
            // Arrange
            //Precondicion el Usuario deberia estar en BD
            var implementacionCallback = new UsuarioSesionCallbackImpl();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            // Act
            manejador.ObtenerSessionJugador(usuario);

            // Assert
            Assert.IsTrue(implementacionCallback.SesionAbierta, "El callback debería haber sido llamado y la sesión debería estar activa.");
            Assert.AreEqual(CommunicationState.Opened, ((ICommunicationObject)implementacionCallback).State, "El canal debería estar en estado abierto.");
            Assert.IsTrue(manejador.YaIniciadoSesion(usuario.Nombre), "El canal debería estar en estado abierto.");

            implementacionCallback.Close();

        }
        [TestMethod]
        public void ObtenerSessionJugadorCallback_YaHaIniciadoSesion_DeberiaRetornarUsuarioFalla()
        {
            // Arrange

            var implementacionCallback = new UsuarioSesionCallbackImpl();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            var usuarioRepetido = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            // Act: 
            manejador.ObtenerSessionJugador(usuario);

            //Assert
            var excepcion = Assert.ThrowsException<FaultException<UsuarioFalla>>(() => manejador.ObtenerSessionJugador(usuarioRepetido));
            Assert.IsTrue(excepcion.Detail.EstaConectado, "La excepción debería indicar que el usuario ya está conectado.");
            implementacionCallback.Close();
        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_EnDesconeccion_DeberiaRetornarFalse()
        {
            // Arrange

            var implementacionCallback = new UsuarioSesionCallbackImpl();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };

            // Act: Llamar a ObtenerSessionJugador una vez para establecer la sesión
            manejador.ObtenerSessionJugador(usuario);
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
            // Arrange

            var implementacionCallback = new UsuarioSesionCallbackImpl();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallback);

            Usuario usuario = null;

            // Act: Llamar a ObtenerSessionJugador una vez para establecer la sesión
            manejador.ObtenerSessionJugador(usuario);

            //Assert
            Assert.IsFalse(implementacionCallback.SesionAbierta, "El callback no debería haber sido llamado");
            if (implementacionCallback != null)
            {
                implementacionCallback.Abort();
            }

        }
        
    }

}

