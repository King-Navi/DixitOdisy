using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
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
            //Pre condicion, el usuario debe exisitir en BD
            string gamertagValido = "unaay";
            string contraseniaValida = "b7a88e8d61d649a44848a48c8de0e6bd48d2fd4d7a61cb733301634d5eac5080";
            var usuarioEsperado = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagValido,
                idUsuario = 2
            };


            // Act
            var resultado = manejador.ValidarCredenciales(gamertagValido, contraseniaValida);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(usuarioEsperado.gamertag, resultado.Nombre, "El nombre del usuario debería coincidir.");
            Assert.AreEqual(usuarioEsperado.idUsuario, resultado.IdUsuario, "El ID del usuario debería coincidir.");
        }
        [TestMethod]
        public void ValidarCredenciales_CredencialesIncorrectas_DeberiaRetornarNulo()
        {
            // Arrange
            string gamertagInvalido = "UsuarioInvalidoParaPruebas";
            string contraseniaInvalida = "ContraseniaIncorrecta123";

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
        [TestMethod]
        public void ValidarCredenciales_ValorNulo_DeberiaRetornarNulo()
        {
            // Arrange
            string gamertagInvalido = null;
            string contraseniaInvalida = null;

            // Act
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            // Arrange
            //Pre condicion, el usuario debe exisitir en BD
            var usuarioEditado = new Usuario
            {
                IdUsuario = 4,  //ID de un usuario existente
                Nombre = "ivan",
                Correo = $"NaviKing{new Random().Next(1000, 9999)}@editado.com", // Correo aleatorio para evitar duplicados
                FotoUsuario = new MemoryStream(new byte[] { 0x20, 0x21, 0x22, 0x23 }), // Ejemplo de foto como MemoryStream
                ContraseniaHASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B" // Hash de la contraseña actualizado
            };

            // Act
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CunadoNoModificoNada_RetornaFalse()
        {
            // Arrange
            //Pre condicion, el usuario debe exisitir en BD
            var usuarioEditado = new Usuario
            {
                IdUsuario = 1, // ID de un usuario existente
                Nombre = "egege",
                Correo = null,
                FotoUsuario = null,
                ContraseniaHASH = null
            };

            // Act
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CunadoTodoEsNulo_RetornaFalse()
        {
            // Arrange
            var usuarioEditado = new Usuario();

            // Act
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoFormatoInvalido_RetornaFalse()
        {
            // Arrange
            string gamertagValido = "juanZZZ";
            string nuevaContraseniaInvalida = "contraseñaNoValida"; 

            // Act
            bool resultado = manejador.EditarContraseniaUsuario(gamertagValido, nuevaContraseniaInvalida);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando la contraseña no tiene el formato SHA-256.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoDatosNulos_RetornaFalse()
        {
            // Arrange
            string gamertagNulo = null;
            string nuevaContraseniaNula = null;

            // Act
            bool resultado = manejador.EditarContraseniaUsuario(gamertagNulo, nuevaContraseniaNula);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando los valores de gamertag y nuevaContrasenia son nulos.");
        }
        [TestMethod]
        public void PingBD_CuandoHayConexion_RetornaTrue()
        {
            //Arrage
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            //Act
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            //Assert
            Assert.IsTrue(respuesta.Result, "El resutlado es true");
        }
        [TestMethod]
        public void PingBD_CuandoNoHayConexion_RetornaFalse()
        {
            //Arrage
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "@-");
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if (!(bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            //Act
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            //Assert
            Assert.IsFalse(respuesta.Result, "El resutlado es falase");
        }
    }
}
