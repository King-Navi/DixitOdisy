using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{

    [TestClass]
    public class ServicioUsuario_Prueba : ConfiguradorPruebaParaServicio
    {
        
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarUsuarioDAO
            .Setup(dao => dao.EditarUsuario(It.IsAny<UsuarioPerfilDTO>()))
            .Returns((UsuarioPerfilDTO usuarioEditado) =>
                {
                    if (usuarioEditado == null ||
                        usuarioEditado.IdUsuario <= 0 ||
                        string.IsNullOrEmpty(usuarioEditado.NombreUsuario) ||
                        usuarioEditado.NombreUsuario.ToLower().Contains(PALABRA_PROHIBIDA_GUEST))
                    {
                        return false;
                    }
                    return true;
                });
        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        [TestMethod]
        public void EditarUsuario_CuandoDatosValidos_DeberiaActualizarUsuario()
        {
            
            var usuarioEditado = new WcfServicioLibreria.Modelo.Usuario
            {
                IdUsuario = 4, 
                Nombre = "ivan",
                Correo = $"NaviKing{new Random().Next(1000, 9999)}@editado.com", 
                FotoUsuario = new MemoryStream(new byte[] { 0x20, 0x21, 0x22, 0x23 }),
                ContraseniaHASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B"
            };

            
            bool resultado = manejador.EditarUsuario(usuarioEditado);

            
            Assert.IsTrue(resultado, "El método debería retornar true cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CuandoRegesaFalse_RetornaFalse()
        {
            
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

            
            bool resultado = manejador.EditarUsuario(usuario);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }
        [TestMethod]
        public void EditarUsuario_CunadoTodoNull_RetornaFalse()
        {
            

            
            bool resultado = manejador.EditarUsuario(null);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando se actualiza el usuario con datos válidos.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoTodoValido_RetornaTrue()
        {
            
            string gamertagValido = "leonardo";
            string nuevaContraseniaValida = "4327FE7955FF63FCF809A48C130B3546EFF6FEF893A396B4FEE083E853C0BA5C";

            
            bool resultado = manejador.EditarContraseniaUsuario(gamertagValido, nuevaContraseniaValida);

            
            Assert.IsFalse(resultado, "El método debería retornar true cuando la contraseña tiene el formato SHA-256.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoFormatoInvalido_RetornaFalse()
        {
            
            string gamertagValido = "juanZZZ";
            string nuevaContraseniaInvalida = "contraseñaNoValida"; 

            
            bool resultado = manejador.EditarContraseniaUsuario(gamertagValido, nuevaContraseniaInvalida);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando la contraseña no tiene el formato SHA-256.");
        }

        [TestMethod]
        public void EditarContraseniaUsuario_CuandoDatosNulos_RetornaFalse()
        {
            
            string gamertagNulo = null;
            string nuevaContraseniaNula = null;

            
            bool resultado = manejador.EditarContraseniaUsuario(gamertagNulo, nuevaContraseniaNula);

            
            Assert.IsFalse(resultado, "El método debería retornar false cuando los valores de gamertag y nuevaContrasenia son nulos.");
        }
        [TestMethod]
        public void PingBD_CuandoHayConexion_RetornaTrue()
        {
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            Assert.IsTrue(respuesta.Result, "El resutlado es true");
        }
        [TestMethod]
        public void PingBD_CuandoNoHayConexion_RetornaFalse()
        {
            var respuesta= Task.Run(async() => await manejador.PingBDAsync());
            Assert.IsFalse(respuesta.Result, "El resutlado es falase");
        }
        # region Ping
        [TestMethod]
        public void Ping_DeberiaRetornarTrue()
        {
            var resultado = manejador.Ping();
            Assert.IsTrue(resultado);
        }

        #endregion

        #region DesconectarUsuario
        [TestMethod]
        public async Task DesconectarUsuario_UsuarioConectado_DeberiaRemoverUsuarioDelDiccionario()
        {
            var usuario = new WcfServicioLibreria.Modelo.Usuario();
            usuario.IdUsuario = 1;
            usuario.Nombre = "Juan";
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario()
            {
                idUsuario = 1
            });
            manejador.ObtenerSesionJugador(usuario);
            implementacionCallback.Close();
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsFalse(manejador.YaIniciadoSesion(usuario.Nombre));
        }

        [TestMethod]
        public async Task DesconectarUsuario_UsuarioNoConectado_NoDeberiaRemoverUsuarioDelDiccionario()
        {
            var usuario = new WcfServicioLibreria.Modelo.Usuario();
            usuario.IdUsuario = 2;
            usuario.Nombre = "Pedro";
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns((DAOLibreria.ModeloBD.Usuario)null);
            implementacionCallback.Close();
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsFalse(manejador.YaIniciadoSesion(usuario.Nombre));
        }

        [TestMethod]
        public async Task DesconectarUsuario_UsuarioConectado_DeberiaDispararEventos()
        {
            var usuario = new WcfServicioLibreria.Modelo.Usuario();
            usuario.IdUsuario = 3;
            usuario.Nombre = "Maria";
            imitarUsuarioDAO.Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>())).Returns(new DAOLibreria.ModeloBD.Usuario()
            {
                idUsuario = 3
            });
            bool eventoClosedLlamado = false;
            implementacionCallback.Closed += (emisor, evento) => eventoClosedLlamado = true;
            manejador.ObtenerSesionJugador(usuario);
            implementacionCallback.Close();
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsTrue(eventoClosedLlamado);
            Assert.IsFalse(manejador.YaIniciadoSesion(usuario.Nombre));
        }



        #endregion



    }
}
