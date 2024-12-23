﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
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
            manejador = null;
            base.ConfigurarManejador();
        }

        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
            manejador = null;
        }

        #region ObtenerSessionJugadorCallback
        [TestMethod]
        public void ObtenerSessionJugadorCallback_SeAbreElCanal_DeberiaRetornarTrue()
        {
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario()
            {
                idUsuario = 1,
                gamertag = "PruebaUsuario"
            });
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();
            imitacionContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
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
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario()
            {
                idUsuario = 1,
                gamertag = "PruebaUsuario"
            });
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();
            imitacionContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
             .Returns(implementacionCallback);
            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            manejador.ObtenerSesionJugador(usuario);
            implementacionCallback.Close();
            var resultado = manejador.YaIniciadoSesion("PruebaUsuario");
            Assert.IsFalse(resultado, "No deberia estar inciado");
        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_YaHaIniciadoSesion_DeberiaRetornarUsuarioFalla()
        {
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario()
            {
                idUsuario = 1,
                gamertag = "PruebaUsuario"
            });
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();
            imitacionContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())   
                .Returns(implementacionCallback);
            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            var usuarioRepetido = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            manejador.ObtenerSesionJugador(usuario);
            var excepcion = Assert.ThrowsException<FaultException<UsuarioFalla>>(() => manejador.ObtenerSesionJugador(usuarioRepetido));
            Assert.IsTrue(excepcion.Detail.EstaConectado, "La excepción debería indicar que el usuario ya está conectado.");
            implementacionCallback.Close();
        }

        [TestMethod]
        public void ObtenerSessionJugadorCallback_EnDesconeccion_DeberiaRetornarFalse()
        {
            var implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();
            imitacionContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
                .Returns(implementacionCallback);
            var usuario = new Usuario { IdUsuario = 1, Nombre = "PruebaUsuario" };
            manejador.ObtenerSesionJugador(usuario);
            implementacionCallback.Abort();
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
            imitacionContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
                .Returns(implementacionCallback);
            Usuario usuario = null;
            manejador.ObtenerSesionJugador(usuario);
            Assert.IsFalse(implementacionCallback.SesionAbierta, "El callback no debería haber sido llamado");
            if (implementacionCallback != null)
            {
                implementacionCallback.Abort();
            }
        }
        #endregion ObtenerSessionJugadorCallback

        # region ConectarUsuario
        [TestMethod]
        public void ConectarUsuario_UsuarioNuevo_DeberiaRetornarTrue()
        {
            var usuario = new Usuario();
            usuario.IdUsuario = 1;
            var resultado = manejador.ConectarUsuario(usuario);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void ConectarUsuario_UsuarioExistente_DeberiaRetornarFalse()
        {
            var usuario = new Usuario();
            usuario.IdUsuario = 1;
            var usuarioRepetido = new Usuario();
            usuarioRepetido.IdUsuario = 1;
            manejador.ConectarUsuario(usuario);
            var resultado = manejador.ConectarUsuario(usuarioRepetido);
            Assert.IsFalse(resultado);
        }


        #endregion

    }

}

