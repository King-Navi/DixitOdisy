using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using Pruebas.ServidorDescribeloPrueba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioRegistro
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
        public void RegistrarUsuario_FaltanCampos_RetornaFalse()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Nombre = "Pedro57Master";
            //Act
            bool resultado = manejador.RegistrarUsuario(usuario);
            //Result
            Assert.IsFalse(resultado, "El usuario no deberia ser registrado");
        }
        [TestMethod]
        public void RegistrarUsuario_SHA256Invalido_RetornaFalse()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Nombre = "Pedro57Master";
            usuario.ContraseniaHASH = "Invalido";
            usuario.Correo = "CorreoValido@gmail.com";
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            //Act
            bool resultado = manejador.RegistrarUsuario(usuario);
            //Result
            Assert.IsFalse(resultado, "El usuario no deberia ser registrado");
        }
        [TestMethod]
        public void RegistrarUsuario_RegistroExistoso_RetornaTrue()
        {
            //Arrage
            Usuario usuario = GeneradorAleatorio.GenerarUsuarioAleatorio();
            //Act
            bool resultado = manejador.RegistrarUsuario(usuario);
            //Result
            Assert.IsTrue(resultado, "El usuario no deberia ser registrado");
        }

    }
}
