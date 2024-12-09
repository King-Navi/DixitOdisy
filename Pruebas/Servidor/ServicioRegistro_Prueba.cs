using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioRegistro_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }
        #region RegistrarUsuario
        [TestMethod]
        public void RegistrarUsuario_FaltanCampos_RetornaFalse()
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = "Pedro57Master";
            bool resultado = manejador.RegistrarUsuario(usuario);
            Assert.IsFalse(resultado, "El usuario no deberia ser registrado");
        }
        [TestMethod]
        public void RegistrarUsuario_SHA256Invalido_RetornaFalse()
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = "Pedro57Master";
            usuario.ContraseniaHASH = "Invalido";
            usuario.Correo = "CorreoValido@gmail.com";
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            bool resultado = manejador.RegistrarUsuario(usuario);
            Assert.IsFalse(resultado, "El usuario no deberia ser registrado");
        }
        [TestMethod]
        public void RegistrarUsuario_RegistroExistoso_RetornaTrue()
        {
            imitarUsuarioDAO.Setup(dao => dao.RegistrarNuevoUsuario(It.IsAny<DAOLibreria.ModeloBD.Usuario>(), It.IsAny<DAOLibreria.ModeloBD.UsuarioCuenta>())).Returns(true);


            Usuario usuario = GeneradorAleatorio.GenerarUsuarioAleatorio();
            bool resultado = manejador.RegistrarUsuario(usuario);
            Assert.IsTrue(resultado, "El usuario no deberia ser registrado");
        }

        #endregion
    }
}
