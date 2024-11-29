using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.IO;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuario_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        protected override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarUsuarioDAO
            .Setup(dao => dao.EditarUsuario(It.IsAny<UsuarioPerfilDTO>()))
            .Returns((UsuarioPerfilDTO usuarioEditado) =>
                {
                    if (usuarioEditado == null ||
                        usuarioEditado.IdUsuario <= 0 ||
                        string.IsNullOrEmpty(usuarioEditado.NombreUsuario) ||
                        usuarioEditado.NombreUsuario.ToLower().Contains("guest"))
                    {
                        return false;
                    }
                    return true;
                });
        }
        [TestCleanup]
        protected override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            // Arrange
            var usuarioEditado = new WcfServicioLibreria.Modelo.Usuario
            {
                IdUsuario = 4, 
                Nombre = "ivan",
                Correo = $"NaviKing{new Random().Next(1000, 9999)}@editado.com", 
                FotoUsuario = new MemoryStream(new byte[] { 0x20, 0x21, 0x22, 0x23 }),
                ContraseniaHASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B"
            };

            // Act
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CuandoRegesaFalse_RetornaFalse()
        {
            // Arrange
            var usuario = new WcfServicioLibreria.Modelo.Usuario
            {
                IdUsuario = 4,
                Nombre = "ivan",
                Correo = $"NaviKing{new Random().Next(1000, 9999)}@editado.com",
                FotoUsuario = new MemoryStream(new byte[] { 0x20, 0x21, 0x22, 0x23 }),
                ContraseniaHASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B"
            };
            imitarUsuarioDAO
             .Setup(dao => dao.EditarUsuario(It.IsAny<UsuarioPerfilDTO>()))
             .Returns((UsuarioPerfilDTO usuarioEditado) =>
                 {
                     return false;
                 });

            // Act
            bool resultado = manejador.EditarUsuario(usuario);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CunadoTodoNull_RetornaFalse()
        {
            // Arrange

            // Act
            bool resultado = manejador.EditarUsuario(null);

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
            //Act
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            //Assert
            Assert.IsTrue(respuesta.Result, "El resutlado es true");
        }
        [TestMethod]
        public void PingBD_CuandoNoHayConexion_RetornaFalse()
        {
            //Arrage
            //Act
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            //Assert
            Assert.IsFalse(respuesta.Result, "El resutlado es falase");
        }
        

        
    }
}
