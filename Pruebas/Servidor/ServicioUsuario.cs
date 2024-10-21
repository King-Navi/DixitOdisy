using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuario
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
        public void ValidarCredenciales_CredencialesCorrectas_DeberiaRetornarUsuario()
        {
            // Arrange
            string gamertagValido = "NaviKing";
            string contraseniaValida = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B";
            var usuarioEsperado = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagValido,
                idUsuario = 1
            };


            // Act
            var resultado = manejador.ValidarCredenciales(gamertagValido, contraseniaValida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(usuarioEsperado.gamertag, resultado.Nombre, "El nombre del usuario debería coincidir.");
            Assert.AreEqual(usuarioEsperado.idUsuario, resultado.IdUsuario, "El ID del usuario debería coincidir.");
        }
        [TestMethod]
        public void ValidarCredenciales_CredencialesIncorrectas_DeberiaRetornarUsuarioVacio()
        {
            // Arrange
            string gamertagInvalido = "UsuarioInvalidoParaPruebas";
            string contraseniaInvalida = "ContraseniaIncorrecta123";

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un objeto usuario, incluso si es vacío.");
            Assert.IsNull(resultado.Nombre, "El nombre del usuario debería ser nulo o vacío.");
            Assert.AreEqual(0, resultado.IdUsuario, "El ID del usuario debería ser 0 o un valor predeterminado.");
        }
    }
}
