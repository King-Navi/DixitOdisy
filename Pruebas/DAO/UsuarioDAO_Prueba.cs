using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioDAO_Prueba : ConfiguracionPruebaBD
    {
        private Usuario usuarioInicial;
        private const string POR_DEFECTO_NOMBRE = "UsuarioPredeterminado";
        private const string POR_DEFECTO_CORREO = "default@example.com";
        private const string POR_DEFECTO_CONTRANIA_HASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B";
        private static readonly byte[] POR_DEFECTO_FOTO_PERFIL = new byte[] { }; 
        private static readonly DateTime POR_DEFECTO_ULTIMA_CONEXION = DateTime.Now;
        private const int POR_DEFECTO_IDUSUARIO_CUENTA = 1;
        private const string CONTRASENIA_INCORRECTA = "1234";
        private UsuarioDAO usuarioDAO = new UsuarioDAO();

        [TestInitialize]
        public void BuscarUsuarioInicial()
        {
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario
                    .SingleOrDefault(u => u.idUsuario == POR_DEFECTO_IDUSUARIO_CUENTA);

                if (usuario != null)
                {
                    usuarioInicial = usuario;
                }
                else
                {
                    Assert.Fail("No se encontro el usuario inicial");
                }
            }
        }
        [TestCleanup]
        public void LimpiarUsuarioInicial()
        {
            usuarioInicial = null;
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario
                    .SingleOrDefault(u => u.idUsuario == POR_DEFECTO_IDUSUARIO_CUENTA);

                if (usuario != null)
                {
                    usuario.gamertag = POR_DEFECTO_NOMBRE;
                    usuario.fotoPerfil = POR_DEFECTO_FOTO_PERFIL;
                    usuario.ultimaConexion = POR_DEFECTO_ULTIMA_CONEXION;
                    usuario.idUsuarioCuenta = POR_DEFECTO_IDUSUARIO_CUENTA;
                    context.SaveChanges();
                }
            }
        }

        #region RegistrarNuevoUsuario
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsCoinciden_DeberiaRegistrar()
        {
            
           var usuario = Utilidad.GenerarUsuarioDePrueba();
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba(usuario.gamertag);
            
            bool resultadoPrueba = usuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            
            Assert.IsTrue(resultadoPrueba, "El registro debería haber sido exitoso porque los gamertags coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsNoCoinciden_DeberiaFallar()
        {
            
            var usuario = Utilidad.GenerarUsuarioDePrueba();
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);
            
            bool resultadoPrueba = usuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque los gamertags no coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioEsNulo_DeberiaFallar()
        {
            
            Usuario usuario = null;
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);

            
            bool resultadoPrueba = usuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);

            
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque Usuario es nulo.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioYaExiste_DeberiaFallar()
        {
            
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
            
            bool resultado = false;
            
            Assert.ThrowsException<GamertagDuplicadoException>(() => resultado = usuarioDAO.RegistrarNuevoUsuario(nuevoUsuario, nuevoUsuarioCuenta));
            Assert.IsFalse(resultado, "El registro no debería ser exitoso porque el usuario ya existe en la base de datos.");
        }
        #endregion

        #region ObtenerUsuarioPorNombre

        #endregion

        #region EditarUsuario
        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            
            //Pre condicion el usuario debe existir
            string correoAleatorio = $"ivan{new Random().Next(1000, 9999)}@ejemplo.com";
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = POR_DEFECTO_IDUSUARIO_CUENTA, 
                NombreUsuario = POR_DEFECTO_NOMBRE, 
                Correo = correoAleatorio,
                FotoPerfil = new byte[] { 0x20, 0x21, 0x22, 0x23 }, 
                HashContrasenia = POR_DEFECTO_CONTRANIA_HASH
            };

            
            bool resultado = usuarioDAO.EditarUsuario(usuarioEditado);

            
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
            
            //Pre condicion el usuario debe existir
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = POR_DEFECTO_IDUSUARIO_CUENTA,
                NombreUsuario = POR_DEFECTO_NOMBRE,
                Correo = POR_DEFECTO_CORREO,
                FotoPerfil = POR_DEFECTO_FOTO_PERFIL,
                HashContrasenia = POR_DEFECTO_CONTRANIA_HASH
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

            
            bool resultado = usuarioDAO.EditarUsuario(usuarioEditado);

            
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
            
            var usuarioEditado = new UsuarioPerfilDTO
            {
                IdUsuario = POR_DEFECTO_IDUSUARIO_CUENTA,
                NombreUsuario = POR_DEFECTO_NOMBRE
            };

            
            bool resultado = usuarioDAO.EditarUsuario(usuarioEditado);

            
            Assert.IsFalse(resultado, "El método debería retornar true al no actulizar nada.");

        }
        #endregion

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CuandoCredencialesSonValidas_DeberiaRetornarUsuario()
        {
            

            
            UsuarioPerfilDTO usuario = usuarioDAO.ValidarCredenciales(POR_DEFECTO_NOMBRE, POR_DEFECTO_CONTRANIA_HASH);
            
            Assert.IsNotNull(usuario, "El usuario debería ser retornado cuando las credenciales son válidas.");
            Assert.AreEqual(POR_DEFECTO_NOMBRE, usuario.NombreUsuario, "El gamertag del usuario retornado debería coincidir con el gamertag proporcionado.");
        }

        [TestMethod]
        public void ValidarCredenciales_CuandoContraseniaEsIncorrecta_DeberiaRetornarNull()
        {
            

            
            UsuarioPerfilDTO usuario = usuarioDAO.ValidarCredenciales(POR_DEFECTO_NOMBRE, CONTRASENIA_INCORRECTA);

            
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando la contraseña es incorrecta.");
        }
        [TestMethod]
        public void ValidarCredenciales_CuandoEsNulo_DeberiaRetornarNull()
        {
            
            
            UsuarioPerfilDTO usuario = usuarioDAO.ValidarCredenciales(null, null);

            
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando el gamertag no existe en la base de datos.");
        }
        #endregion ValidarCredenciales

        #region ColocarUltimaConexion
        
        [TestMethod]
        public void ColocarUltimaConexion_UsuarioExistente_DeberiaActualizarUltimaConexion()
        {
            
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioExistente = 1;
            DateTime antesDeLlamar = DateTime.Now;

            
            bool resultado = usuarioDAO.ColocarUltimaConexion(idUsuarioExistente);

            
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
            
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioInexistente = -1; 

            
            bool resultado = usuarioDAO.ColocarUltimaConexion(idUsuarioInexistente);

            
            Assert.IsFalse(resultado, "El método debería retornar false para un usuario inexistente.");
        }
        [TestMethod]
        public void ColocarUltimaConexion_IdCero_DeberiaRetornarFalse()
        {
            
            // Precondición: Este ID no debe existir en la base de datos
            int idUsuarioInexistente = 0; 

            
            bool resultado = usuarioDAO.ColocarUltimaConexion(idUsuarioInexistente);

            
            Assert.IsFalse(resultado, "El método debería retornar false para un usuario inexistente.");
        }

        #endregion ColocarUltimaConexion

        # region ObtenerUsuarioPorId

        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdExiste_DeberiaRetornarUsuario()
        {
            
            // Un ID que existe en la base de datos

            
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_VALIDO);

            
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(ID_VALIDO, resultado.idUsuario, "El ID del usuario debería coincidir.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdNoExiste_DeberiaRetornarNull()
        {
            
            // Un ID que no existe en la base de datos

            
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_INEXISTENTE);

            
            Assert.IsNull(resultado, "El método debería devolver null cuando el ID no existe.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdEsInvalido_DeberiaRetornarNull()
        {
            
            // Un ID inválido
            
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_INVALIDO);

            
            Assert.IsNull(resultado, "El método debería devolver null cuando el ID es inválido.");
        }

        #endregion ObtenerUsuarioPorId

        
    }
}
