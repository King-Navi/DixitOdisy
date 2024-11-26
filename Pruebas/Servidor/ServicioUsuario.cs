using DAOLibreria;
using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuario
    {
        private const int ID_INVALIDO = -1;
        private const int ID_INEXISTE = 9999;
        private const int ID_VALIDO = 1;
        private Mock<IContextoOperacion> mockContextoProvedor;
        private ManejadorPrincipal manejador;

        [TestInitialize]
        public void PruebaConfiguracion()
        {
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }

        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            // Arrange
            //Pre condicion, el usuario debe exisitir en BD
            var usuarioEditado = new WcfServicioLibreria.Modelo.Usuario
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
            var usuarioEditado = new WcfServicioLibreria.Modelo.Usuario
            {
                // ID de un usuario existente
                IdUsuario = 1, 
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
            var usuarioEditado = new WcfServicioLibreria.Modelo.Usuario();

            // Act
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoTodoValido_RetornaTrue()
        {
            // Arrange
            string gamertagValido = "leonardo";
            string nuevaContraseniaValida = "4327FE7955FF63FCF809A48C130B3546EFF6FEF893A396B4FEE083E853C0BA5C";

            // Act
            bool resultado = manejador.EditarContraseniaUsuario(gamertagValido, nuevaContraseniaValida);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar true cuando la contraseña tiene el formato SHA-256.");
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
            if (resultado)
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
            if (!resultado)
            {
                Assert.Fail("La BD no está configurada.");
            }
            //Act
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            //Assert
            Assert.IsFalse(respuesta.Result, "El resutlado es falase");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdExiste_DeberiaRetornarUsuario()
        {
            // Arrange
            // Un ID que existe en la base de datos

            // Act
            DAOLibreria.ModeloBD.Usuario resultado = UsuarioDAO.ObtenerUsuarioPorId(ID_VALIDO);

            // Assert
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(ID_VALIDO, resultado.idUsuario, "El ID del usuario debería coincidir.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdNoExiste_DeberiaRetornarNull()
        {
            // Arrange
            // Un ID que no existe en la base de datos

            // Act
            DAOLibreria.ModeloBD.Usuario resultado = UsuarioDAO.ObtenerUsuarioPorId(ID_INEXISTE);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver null cuando el ID no existe.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdEsInvalido_DeberiaRetornarNull()
        {
            // Arrange
            // Un ID inválido
            // Act
            DAOLibreria.ModeloBD.Usuario resultado = UsuarioDAO.ObtenerUsuarioPorId(ID_INVALIDO);

            // Assert
            Assert.IsNull(resultado, "El método debería devolver null cuando el ID es inválido.");
        }
    }
}
