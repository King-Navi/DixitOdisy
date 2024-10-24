using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioCorreo
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
        public void TestCorreo()
        {
            //Arrage
            Usuario usuario = new Usuario();
            usuario.Nombre = "unaay";
            usuario.ContraseniaHASH = "Invalido";
            usuario.Correo = "unaayjose@gmail.com";
            usuario.FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20);
            //Act usuario con mi correo y llamar al metodo
            bool result = manejador.VerificarCorreo(usuario);
            //Result
            Assert.IsTrue(result,"El código ha sido enviado al correo");
        }
    }
}
