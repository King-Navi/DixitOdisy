using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo.Excepciones;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioDAO_Prueba : ConfiguracionPruebaBD
    {
        #region RegistrarNuevoUsuario
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsCoinciden_DeberiaRegistrar()
        {
            // Arrange
           var usuario = Utilidad.GenerarUsuarioDePrueba();
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba(usuario.gamertag);
            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            // Assert
            Assert.IsTrue(resultadoPrueba, "El registro debería haber sido exitoso porque los gamertags coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsNoCoinciden_DeberiaFallar()
        {
            // Arrange
            var usuario = Utilidad.GenerarUsuarioDePrueba();
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);
            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            // Assert
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque los gamertags no coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioEsNulo_DeberiaFallar()
        {
            // Arrange
            Usuario usuario = null;
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);

            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);

            // Assert
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque Usuario es nulo.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioYaExiste_DeberiaFallar()
        {
            // Arrange
            //Pre-condicion: El usuario debe estar ya registrado en base de datos
            var (usuarioExistente, usuarioCuentaExistente) = Utilidad.PrepararUsuarioExistente();

            var nuevoUsuario = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = usuarioExistente.gamertag,
                fotoPerfil = Utilidad.GenerarBytesAleatorios(256) 
            };
            var nuevoUsuarioCuenta = new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = usuarioExistente.gamertag,
                hashContrasenia = Utilidad.ObtenerSHA256Hash("NuevaContraseña"),
                correo = "nuevo@ejemplo.com" 
            };
            // Act
            bool resultado = false;
            // Assert
            Assert.ThrowsException<GamertagDuplicadoException>(() => resultado = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(nuevoUsuario, nuevoUsuarioCuenta));
            Assert.IsFalse(resultado, "El registro no debería ser exitoso porque el usuario ya existe en la base de datos.");
        }
        #endregion

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CuandoCredencialesSonValidas_DeberiaRetornarUsuario()
        {
            // Arrange
            
            //Precondicion: Debe ser un UsuarioCuenta existente en base de datos
            string gamertagValido = "unaay";
            string contraseniaValida = "B7A88E8D61D649A44848A48C8DE0E6BD48D2FD4D7A61CB733301634D5EAC5080";
            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagValido, contraseniaValida);
            // Assert
            Assert.IsNotNull(usuario, "El usuario debería ser retornado cuando las credenciales son válidas.");
            Assert.AreEqual(gamertagValido, usuario.NombreUsuario, "El gamertag del usuario retornado debería coincidir con el gamertag proporcionado.");
        }

        [TestMethod]
        public void ValidarCredenciales_CuandoContraseniaEsIncorrecta_DeberiaRetornarNull()
        {
            // Arrange
            

            //Debe ser un UsuarioCuenta NO existente en base de datos
            string gamertagValido = "NaviKing";
            string contraseniaInvalida = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4a";

            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagValido, contraseniaInvalida);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando la contraseña es incorrecta.");
        }
        [TestMethod]
        public void ValidarCredenciales_CuandoEsNulo_DeberiaRetornarNull()
        {
            // Arrange
            string gamertagInvalido= null;
            string contrasenia = null; // No importa el hash porque el gamertag no existe.

            // Act
            UsuarioPerfilDTO usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagInvalido, contrasenia);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando el gamertag no existe en la base de datos.");
        }
        #endregion

        #region ObtenerUsuarioPorNombre
        
        #endregion

        #region  
        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            // Arrange
            //Pre condicion el usuario debe existir
            string correoAleatorio = $"ivan{new Random().Next(1000, 9999)}@ejemplo.com";
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = 4, // Un ID existente en el contexto simulado o BD de prueba
                NombreUsuario = "ivan", //Un nombre existente en BD
                Correo = correoAleatorio,
                FotoPerfil = new byte[] { 0x20, 0x21, 0x22, 0x23 }, // Ejemplo de bytes para la nueva foto
                HashContrasenia = "3E203FE617527077E6B2A2ABFF345FAC15CA2E7338A2D30BF75AD3E5F49504C1"
            };

            // Act
            bool resultado = UsuarioDAO.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true al actualizar el usuario con datos válidos.");

            // Verificar que los cambios se aplicaron correctamente
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.Single(u => u.idUsuario == usuarioEditado.IdUsuario);
                var usuarioCuenta = context.UsuarioCuenta.Single(uc => uc.gamertag == usuarioEditado.NombreUsuario);

                Assert.AreEqual(usuarioEditado.Correo, usuarioCuenta.correo, "El correo del usuario debería haberse actualizado.");
                CollectionAssert.AreEqual(usuarioEditado.FotoPerfil, usuario.fotoPerfil, "La foto de perfil debería haberse actualizado.");
                Assert.AreEqual(usuarioEditado.HashContrasenia, usuarioCuenta.hashContrasenia, "El hash de la contraseña debería haberse actualizado.");
            }
        }
        [TestMethod]
        public void EditarUsuario_ModificacionIgual_DeberiaActualizarUsuario()
        {
            // Arrange
            //Pre condicion el usuario debe existir
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = 4, // Un ID existente en el contexto simulado o BD de prueba
                NombreUsuario = "ivan", //Un nombre existente en BD
                Correo = "unaay8657@ejemplo.com",
                HashContrasenia = null,
                FotoPerfil = null
            };
            string correoAnterior ;
            byte[] fotoAnterior;
            string gamertagUsuario;
            string gamertagUsuarioCuenta;
            string contraseniaAnteriror ;
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.Single(u => u.idUsuario == usuarioEditado.IdUsuario);
                var usuarioCuenta = context.UsuarioCuenta.Single(uc => uc.gamertag == usuarioEditado.NombreUsuario);
                correoAnterior = usuarioCuenta.correo;
                fotoAnterior = usuario.fotoPerfil;
                gamertagUsuario = usuario.gamertag;
                gamertagUsuarioCuenta = usuarioCuenta.gamertag;
                contraseniaAnteriror = usuarioCuenta.hashContrasenia;
            }

            // Act
            bool resultado = UsuarioDAO.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true al no actulizar nada.");

            // Verificar que los cambios se aplicaron correctamente
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.Single(u => u.idUsuario == usuarioEditado.IdUsuario);
                var usuarioCuenta = context.UsuarioCuenta.Single(uc => uc.gamertag == usuarioEditado.NombreUsuario);

                Assert.AreEqual(usuarioEditado.Correo, usuarioCuenta.correo, "El correo del usuario debería haberse actualizado.");
                CollectionAssert.AreEqual(fotoAnterior, usuario.fotoPerfil, "La foto de perfil debería ser la misma.");
                Assert.AreEqual(contraseniaAnteriror, usuarioCuenta.hashContrasenia, "El hash de la contraseña debería ser el mismo.");
                Assert.AreEqual(gamertagUsuario, usuario.gamertag, "El gamertag deberia ser el mismo.");
                Assert.AreEqual(gamertagUsuarioCuenta, usuarioCuenta.gamertag, "El gamertag deberia ser el mismo.");
            }
        }
        [TestMethod]
        public void EditarUsuario_NoModificoNada_RetornaFalse()
        {
            // Arrange
            //Pre condicion el usuario debe existir
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = 4, // Un ID existente en el contexto simulado o BD de prueba
                NombreUsuario = "ivan", //Un nombre existente en BD
                Correo = null,
                HashContrasenia = null,
                FotoPerfil = null
            };
            string correoAnterior ;
            byte[] fotoAnterior;
            string gamertagUsuario;
            string gamertagUsuarioCuenta;
            string contraseniaAnteriror ;
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.Single(u => u.idUsuario == usuarioEditado.IdUsuario);
                var usuarioCuenta = context.UsuarioCuenta.Single(uc => uc.gamertag == usuarioEditado.NombreUsuario);
                correoAnterior = usuarioCuenta.correo;
                fotoAnterior = usuario.fotoPerfil;
                gamertagUsuario = usuario.gamertag;
                gamertagUsuarioCuenta = usuarioCuenta.gamertag;
                contraseniaAnteriror = usuarioCuenta.hashContrasenia;
            }

            // Act
            bool resultado = UsuarioDAO.EditarUsuario(usuarioEditado);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar true al no actulizar nada.");

            // Verificar que los cambios se aplicaron correctamente
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.Single(u => u.idUsuario == usuarioEditado.IdUsuario);
                var usuarioCuenta = context.UsuarioCuenta.Single(uc => uc.gamertag == usuarioEditado.NombreUsuario);

                Assert.AreEqual(correoAnterior, usuarioCuenta.correo, "El correo del usuario debería ser el mismp.");
                CollectionAssert.AreEqual(fotoAnterior, usuario.fotoPerfil, "La foto de perfil debería ser la misma.");
                Assert.AreEqual(contraseniaAnteriror, usuarioCuenta.hashContrasenia, "El hash de la contraseña debería ser el mismo.");
                Assert.AreEqual(gamertagUsuario, usuario.gamertag, "El gamertag deberia ser el mismo.");
                Assert.AreEqual(gamertagUsuarioCuenta, usuarioCuenta.gamertag, "El gamertag deberia ser el mismo.");
            }
        }
        #endregion

        #region VerificarCorreoConGamertag

        [TestMethod]
        public void VerificarCorreoConGamertag_CuandoCoinciden_DeberiaRetornarTrue()
        {
            // Arrange
            var correo = "unaayjose@gmail.com";
            var gamertag = "unaay";

            // Act
            bool resultado = UsuarioDAO.ExisteUnicoUsuarioConGamertagYCorreo(gamertag, correo);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando el correo y el gamertag coinciden.");
        }

        [TestMethod]
        public void VerificarCorreoConGamertag_CuandoNoCoinciden_DeberiaRetornarFalse()
        {
            // Arrange
            var correo = "unaayjose@gmail.com";
            var gamertag = "unaay20";

            // Act
            bool resultado = UsuarioDAO.ExisteUnicoUsuarioConGamertagYCorreo(gamertag, correo);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el correo y el gamertag no coinciden.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoDatosValidos_DeberiaRetornarTrue()
        {
            // Arrange
            string gamertag = "leoleo";
            string nuevoHashContrasenia = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd6d0fcb4c8b9e3fbb5"; // SHA256 de "password"

            // Act
            bool resultado = UsuarioDAO.EditarContraseniaPorGamertag(gamertag, nuevoHashContrasenia);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true cuando los datos son válidos y la contraseña está en SHA256.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoContraseniaNoEsSHA256_DeberiaRetornarFalse()
        {
            // Arrange
            string gamertag = "usuarioPrueba";
            string nuevoHashContraseniaInvalido = "password"; // Contraseña en texto plano, no en SHA256

            // Act
            bool resultado = UsuarioDAO.EditarContraseniaPorGamertag(gamertag, nuevoHashContraseniaInvalido);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando la nueva contraseña no está en SHA256.");
        }

        [TestMethod]
        public void EditarContraseniaPorGamertag_CuandoGamertagYContraseniaSonNulos_DeberiaRetornarFalse()
        {
            // Arrange
            string gamertagNulo = null;
            string nuevoHashContraseniaNulo = null;

            // Act
            bool resultado = UsuarioDAO.EditarContraseniaPorGamertag(gamertagNulo, nuevoHashContraseniaNulo);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false cuando el gamertag y la nueva contraseña son nulos.");
        }
        #endregion VerificarCorreoConGamertag

        #region ColocarUltimaConexion

        [TestMethod]
        public void ColocarUltimaConexion_UsuarioExistente_DeberiaActualizarUltimaConexion()
        {
            // Arrange
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioExistente = 1;
            DateTime antesDeLlamar = DateTime.Now;

            // Act
            bool resultado = DAOLibreria.DAO.UsuarioDAO.ColocarUltimaConexion(idUsuarioExistente);

            // Assert
            Assert.IsTrue(resultado, "El método debería retornar true para un usuario existente.");

            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.FirstOrDefault(u => u.idUsuario == idUsuarioExistente);
                Assert.IsNotNull(usuario, "El usuario debería existir en la base de datos.");
                Assert.IsTrue(usuario.ultimaConexion >= antesDeLlamar, "La fecha de última conexión no se actualizó correctamente.");
            }
        }

        [TestMethod]
        public void ColocarUltimaConexion_UsuarioInexistente_DeberiaRetornarFalse()
        {
            // Arrange
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioInexistente = -1; 

            // Act
            bool resultado = DAOLibreria.DAO.UsuarioDAO.ColocarUltimaConexion(idUsuarioInexistente);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false para un usuario inexistente.");
        }
        [TestMethod]
        public void ColocarUltimaConexion_IdCero_DeberiaRetornarFalse()
        {
            // Arrange
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioInexistente = 0; 

            // Act
            bool resultado = DAOLibreria.DAO.UsuarioDAO.ColocarUltimaConexion(idUsuarioInexistente);

            // Assert
            Assert.IsFalse(resultado, "El método debería retornar false para un usuario inexistente.");
        }

        #endregion
    }
}
