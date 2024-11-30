using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.DAO.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int LONGITUD_CADENA = 100;
        private const int CERO = 0;

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
            var nuevoUsuario = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = POR_DEFECTO_NOMBRE,
                fotoPerfil = Utilidad.GenerarBytesAleatorios(256) 
            };
            var nuevoUsuarioCuenta = new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = POR_DEFECTO_NOMBRE,
                hashContrasenia = Utilidad.ObtenerSHA256Hash(NUEVA_CONTRASENIA),
                correo = "nuevo@ejemplo.com" 
            };
            bool resultado = false;
            Assert.ThrowsException<GamertagDuplicadoException>(() => resultado = usuarioDAO.RegistrarNuevoUsuario(nuevoUsuario, nuevoUsuarioCuenta));
            Assert.IsFalse(resultado, "El registro no debería ser exitoso porque el usuario ya existe en la base de datos.");
        }
        #endregion

        #region ObtenerUsuarioPorNombre
        [TestMethod]
        public void ObtenerUsuarioPorNombre_UsuarioExistente_DeberiaRetornarUsuario()
        {
            var usuario = usuarioInicial; 
            var usuarioConsulta = usuarioDAO.ObtenerUsuarioPorNombre(usuarioInicial.gamertag);
            Assert.IsNotNull(usuarioConsulta);
            Assert.AreEqual(usuarioInicial.gamertag, usuarioConsulta.gamertag);
            Assert.AreEqual(usuarioInicial.idUsuario, usuarioConsulta.idUsuario);
        }

        [TestMethod]
        public void ObtenerUsuarioPorNombre_UsuarioInexistente_DeberiaRetornarNull()
        {
            var gamertag = NOMBRE_USUARIO_INEXISTENTE;
            var usuario = usuarioDAO.ObtenerUsuarioPorNombre(gamertag);
            Assert.IsNull(usuario);
        }

        [TestMethod]
        public void ObtenerUsuarioPorNombre_GamertagNull_DeberiaManejarExcepcion()
        {
            string gamertag = null;
            Usuario usuario = null;
            usuario = usuarioDAO.ObtenerUsuarioPorNombre(gamertag);
            Assert.IsNull(usuario);
        }


        #endregion ObtenerUsuarioPorNombre

        #region  ObtenerIdPorNombre

        [TestMethod]
        public void ObtenerIdPorNombre_UsuarioExistente_DeberiaRetornarId()
        {

            var idObtenido = usuarioDAO.ObtenerIdPorNombre(usuarioInicial.gamertag);
            Assert.AreEqual(usuarioInicial.idUsuario, idObtenido);
        }

        [TestMethod]
        public void ObtenerIdPorNombre_UsuarioInexistente_DeberiaRetornarCero()
        {
            var gamertag = NOMBRE_USUARIO_INEXISTENTE;
            var idObtenido = usuarioDAO.ObtenerIdPorNombre(gamertag);
            Assert.AreEqual(0, idObtenido);
        }
        [TestMethod]
        public void ObtenerIdPorNombre_GamertagNull_DeberiaRetornarCero()
        {
            string gamertag = null;
            var idObtenido = usuarioDAO.ObtenerIdPorNombre(gamertag);
            Assert.AreEqual(CERO, idObtenido);
        }

        #endregion ObtenerIdPorNombre

        # region ObtenerListaUsuariosPorNombres

        [TestMethod]
        public void ObtenerListaUsuariosPorNombres_UsuariosExistentes_DeberiaRetornarListaUsuarios()
        {
            var nombres = new List<string> { POR_DEFECTO_NOMBRE_SEGUNDO, 
                POR_DEFECTO_NOMBRE_TERCERO, 
                POR_DEFECTO_NOMBRE};
            var usuarios = usuarioDAO.ObtenerListaUsuariosPorNombres(nombres);
            Assert.AreEqual(nombres.Count, usuarios.Count);
            foreach (var nombre in nombres)
            {
                Assert.IsTrue(usuarios.Any(u => u.gamertag == nombre));
            }
        }

        [TestMethod]
        public void ObtenerListaUsuariosPorNombres_UsuariosInexistentes_DeberiaRetornarListaVacia()
        {
            var nombres = new List<string> { NOMBRE_USUARIO_INEXISTENTE, NOMBRE_USUARIO_INEXISTENTE + "2"};
            var usuarios = usuarioDAO.ObtenerListaUsuariosPorNombres(nombres);
            Assert.AreEqual(0, usuarios.Count);
        }

        [TestMethod]
        public void ObtenerListaUsuariosPorNombres_ListaNombresNull_DeberiaRetornarListaVacia()
        {
            var dao = new UsuarioDAO();
            List<string> nombres = null;
            var usuarios = dao.ObtenerListaUsuariosPorNombres(nombres);
            Assert.AreEqual(0, usuarios.Count);
        }

        #endregion

        #region VerificarNombreUnico
        [TestMethod]
        public void VerificarNombreUnico_GamertagNoExistente_NoLanzaExcepcion()
        {
            var gamertag = NOMBRE_USUARIO_INEXISTENTE;
            usuarioDAO.VerificarNombreUnico(gamertag);
        }

        [TestMethod]
        [ExpectedException(typeof(GamertagDuplicadoException))]
        public void VerificarNombreUnico_GamertagEnUsuario_LanzaExcepcion()
        {
            var gamertag = POR_DEFECTO_NOMBRE;
            usuarioDAO.VerificarNombreUnico(gamertag);
        }

        [TestMethod]
        [ExpectedException(typeof(GamertagDuplicadoException))]
        public void VerificarNombreUnico_GamertagEnUsuarioCuenta_LanzaExcepcion()
        {
            var dao = new UsuarioDAO();
            var gamertag = POR_DEFECTO_NOMBRE_CUENTA;
            dao.VerificarNombreUnico(gamertag);
        }

        #endregion

        #region ColocarUltimaConexion
        [TestMethod]
        public void ColocarUltimaConexion_UsuarioExistente_DeberiaRetornarTrue()
        {
            var resultado = usuarioDAO.ColocarUltimaConexion(ID_VALIDO);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void ColocarUltimaConexion_UsuarioInexistente_DeberiaRetornarFalse()
        {
            var idUsuario = ID_INEXISTENTE;
            var resultado = usuarioDAO.ColocarUltimaConexion(idUsuario);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void ColocarUltimaConexion_UsuarioExistente_DeberiaActualizarUltimaConexion()
        {
            DateTime antesDeLlamar = DateTime.Now;
            bool resultado = usuarioDAO.ColocarUltimaConexion(ID_VALIDO); Assert.IsTrue(resultado, "El método debería retornar true para un usuario existente.");
            using (var context = new DescribeloEntities())
            {
                var usuario = context.Usuario.FirstOrDefault(u => u.idUsuario == ID_VALIDO);
                Assert.IsNotNull(usuario, "El usuario debería existir en la base de datos.");
                Assert.IsTrue(usuario.ultimaConexion >= antesDeLlamar, "La fecha de última conexión no se actualizó correctamente.");
            }
        }

        [TestMethod]
        public void ColocarUltimaConexion_IdCero_DeberiaRetornarFalse()
        {
            int idUsuarioInexistente = 0;
            bool resultado = usuarioDAO.ColocarUltimaConexion(idUsuarioInexistente); Assert.IsFalse(resultado, "El método debería retornar false para un usuario inexistente.");
        }



        #endregion ColocarUltimaConexion

        #region EditarUsuario
        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
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
        #endregion EditarUsuario
        #region ValidarCredenciales

        [TestMethod]
        public void ValidarCredenciales_CredencialesValidas_DeberiaRetornarUsuarioPerfilDTO()
        {
            var resultado = usuarioDAO.ValidarCredenciales(POR_DEFECTO_NOMBRE_CUENTA, POR_DEFECTO_CONTRANIA_HASH);
            Assert.IsNotNull(resultado);
        }
        [TestMethod]
        public void ValidarCredenciales_GamertagInexistente_DeberiaRetornarNull()
        {
            var ususarioGenerado = Utilidad.GenerarUsuarioCuentaDePrueba(Utilidad.GenerarCadenaAleatoria(LONGITUD_CADENA));
            var resultado = usuarioDAO.ValidarCredenciales(ususarioGenerado.gamertag, ususarioGenerado.hashContrasenia);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValidarCredenciales_ContraseniaIncorrecta_DeberiaRetornarNull()
        {
            var resultado = usuarioDAO.ValidarCredenciales(POR_DEFECTO_NOMBRE_CUENTA, CONTRASENIA_INCORRECTA);
            Assert.IsNull(resultado);
        }

        #endregion ValidarCredenciales

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CuandoCredencialesSonValidas_DeberiaRetornarUsuario()
        {
            UsuarioPerfilDTO usuario = usuarioDAO.ValidarCredenciales(POR_DEFECTO_NOMBRE, POR_DEFECTO_CONTRANIA_HASH);
            Assert.IsNotNull(usuario, "El usuario debería ser retornado cuando las credenciales son válidas.");
            Assert.AreEqual(POR_DEFECTO_NOMBRE, usuario.NombreUsuario, 
                "El gamertag del usuario retornado debería coincidir con el gamertag proporcionado.");
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

        
        # region ObtenerUsuarioPorId

        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdExiste_DeberiaRetornarUsuario()
        {
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_VALIDO);
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(ID_VALIDO, resultado.idUsuario, "El ID del usuario debería coincidir.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdNoExiste_DeberiaRetornarNull()
        {
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_INEXISTENTE);
            Assert.IsNull(resultado, "El método debería devolver null cuando el ID no existe.");
        }
        [TestMethod]
        public void ObtenerUsuarioPorId_CuandoIdEsInvalido_DeberiaRetornarNull()
        {
            DAOLibreria.ModeloBD.Usuario resultado = usuarioDAO.ObtenerUsuarioPorId(ID_INVALIDO);
             Assert.IsNull(resultado, "El método debería devolver null cuando el ID es inválido.");
        }

        #endregion ObtenerUsuarioPorId

        
    }
}
