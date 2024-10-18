using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Cliente.Utilidades;
using Pruebas.ServidorDescribeloPrueba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Cliente
{
    [TestClass]
    public class ServicioRegistro
    {
        [TestMethod]
        public void ObtenerSessionJugadorCallback_SeAbreElCanal_DeberiaRetornarTrue()
        {
            //Pre-Condicion tiene que estar corriendo el servicio
            if (!Conexion.ValidarConexion())
            {
                Assert.Inconclusive("No se pudo establecer la conexión con el servicio. La prueba no puede ejecutarse.");
                return;
            }
            // Arrange
            Usuario usuario = new Usuario();
            usuario.IdUsuario = 1;
            usuario.Nombre = "NaviKing";

            // Crear una instancia de la implementación concreta del callback
            var sesionCallback = new UsuarioSesionCallbackImpl();

            // Act
            bool canal = Conexion.AbrirConexionUsuarioSesionCallback(sesionCallback);
            Conexion.UsuarioSesion.ObtenerSessionJugador(usuario);

            // Assert
            Assert.IsTrue(canal, "El canal debería estar abierto y la sesión activa.");
            Assert.IsTrue(sesionCallback.SesionAbierta, "El callback debería haber sido llamado y la sesión debería estar activa.");
            //------------------------------------------------------------------------------------
            //var mockCallback = new Mock<IServicioUsuarioSesionCallback>();

            //// Configurar el mock para capturar si se llama al método ObtenerSessionJugadorCallback con true
            //bool sesionAbierta = false;
            //mockCallback.Setup(callback => callback.ObtenerSessionJugadorCallback(It.IsAny<bool>()))
            //            .Callback<bool>(abierta => sesionAbierta = abierta);

            //// Act
            //Conexion.AbrirConexionUsuarioSesionCallback(mockCallback.Object);

            //// Assert
            //Assert.IsTrue(sesionAbierta, "El canal debería estar abierto y la sesión activa.");
        }

    }
}
