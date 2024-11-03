using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioCorreo
    {
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;
        private string codigoCorrecto;

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }

        [TestMethod]
        public void TestVerificarCorreoValido()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Nombre = "unaay";
            usuario.ContraseniaHASH = "Invalido";
            usuario.Correo = "unaayjose@gmail.com";
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            //Act 
            bool result = manejador.VerificarCorreo(usuario);
            //Result
            Assert.IsTrue(result,"El código ha sido enviado al correo");
        }

        [TestMethod]
        public void TestVerificarCorreoInvalido()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Nombre = "unaay";
            usuario.ContraseniaHASH = "Invalido";
            usuario.Correo = "unaayjose$gmail,com";
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            //Act 
            bool result = manejador.VerificarCorreo(usuario);
            //Result
            Assert.IsFalse(result, "El código no ha sido enviado al correo");
        }


        [TestMethod]
        public void TestVerificarCodigoCoincide()
        {
            // Arrange
            codigoCorrecto = "ABC123";
            manejador.Codigo = codigoCorrecto;  
            // Act
            bool result = manejador.VerificarCodigo(codigoCorrecto);
            // Assert
            Assert.IsTrue(result, "El código recibido coincide con el generado.");
        }

        [TestMethod]
        public void TestVerificarCodigoNoCoincide()
        {
            // Arrange
            manejador.Codigo = "ABC123";  
            string codigoRecibido = "CodigoInvalido";
            // Act
            bool result = manejador.VerificarCodigo(codigoRecibido);
            // Assert
            Assert.IsFalse(result, "El código recibido no coincide con el generado.");
        }
    }
}
