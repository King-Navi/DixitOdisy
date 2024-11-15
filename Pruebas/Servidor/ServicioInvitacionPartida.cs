using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    public class ServicioInvitacionPartida
    {
        //private ManejadorPrincipal manejador;
        //private ConcurrentDictionary<int, UsuarioContexto> jugadoresConectadosDiccionario;

        //[TestInitialize]
        //public void Setup()
        //{
        //    jugadoresConectadosDiccionario = new ConcurrentDictionary<int, UsuarioContexto>();
        //    manejador = new ManejadorPrincipal()
        //    {
        //        // Propiedad simulada para permitir pruebas
        //        jugadoresConectadosDiccionario = jugadoresConectadosDiccionario
        //    };
        //}

        //[TestMethod]
        //public void EnviarInvitacion_UsuarioConectado_CallbackValido_InvitacionExitosa()
        //{
        //    // Arrange
        //    var callbackMock = new Mock<IInvitacionPartidaCallback>();
        //    var receptor = new UsuarioContexto
        //    {
        //        Nombre = "Receptor",
        //        InvitacionPartidaCallBack = callbackMock.Object
        //    };
        //    jugadoresConectadosDiccionario.TryAdd(1, receptor);

        //    // Act
        //    var resultado = manejador.EnviarInvitacion("Emisor", "Sala123", "Receptor");

        //    // Assert
        //    Assert.IsTrue(resultado);
        //    callbackMock.Verify(c => c.RecibirInvitacion(It.IsAny<InvitacionPartida>()), Times.Once);
        //}

        //[TestMethod]
        //public void EnviarInvitacion_UsuarioNoConectado_InvitacionFalla()
        //{
        //    // Act
        //    var resultado = manejador.EnviarInvitacion("Emisor", "Sala123", "ReceptorInexistente");

        //    // Assert
        //    Assert.IsFalse(resultado);
        //}

        //[TestMethod]
        //public void EnviarInvitacion_CallbackInvalido_InvitacionFalla()
        //{
        //    // Arrange
        //    var receptor = new UsuarioContexto
        //    {
        //        Nombre = "Receptor",
        //        InvitacionPartidaCallBack = null // Callback inválido
        //    };
        //    jugadoresConectadosDiccionario.TryAdd(1, receptor);

        //    // Act
        //    var resultado = manejador.EnviarInvitacion("Emisor", "Sala123", "Receptor");

        //    // Assert
        //    Assert.IsFalse(resultado);
        //}

        //[TestMethod]
        //public void AbrirCanalParaInvitaciones_UsuarioConectado_CanalAbierto()
        //{
        //    // Arrange
        //    var usuario = new Usuario
        //    {
        //        IdUsuario = 1,
        //        Nombre = "UsuarioRemitente"
        //    };
        //    var remitente = new UsuarioContexto { Nombre = "UsuarioRemitente" };
        //    jugadoresConectadosDiccionario.TryAdd(usuario.IdUsuario, remitente);

        //    var callbackMock = new Mock<IInvitacionPartidaCallback>();
        //    var contextoOperacionMock = new Mock<OperationContext>();
        //    contextoOperacionMock.Setup(c => c.GetCallbackChannel<IInvitacionPartidaCallback>())
        //                         .Returns(callbackMock.Object);

        //    manejador.contextoOperacion = contextoOperacionMock.Object;

        //    // Act
        //    manejador.AbrirCanalParaInvitaciones(usuario);

        //    // Assert
        //    Assert.AreEqual(callbackMock.Object, remitente.InvitacionPartidaCallBack);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public void AbrirCanalParaInvitaciones_Exception_CanalNoAbierto()
        //{
        //    // Arrange
        //    var usuario = new Usuario
        //    {
        //        IdUsuario = 1,
        //        Nombre = "UsuarioRemitente"
        //    };
        //    jugadoresConectadosDiccionario.TryAdd(usuario.IdUsuario, new UsuarioContexto { Nombre = "UsuarioRemitente" });

        //    var contextoOperacionMock = new Mock<OperationContext>();
        //    contextoOperacionMock.Setup(c => c.GetCallbackChannel<IInvitacionPartidaCallback>())
        //                         .Throws(new Exception("Error simulado"));
        //    manejador.contextoOperacion = contextoOperacionMock.Object;

        //    // Act
        //    manejador.AbrirCanalParaInvitaciones(usuario);

        //    // Assert (Handled by ExpectedException)
        //}
    }
}
