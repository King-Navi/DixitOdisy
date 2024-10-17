using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.ServidorDescribeloPrueba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.Cliente
{
    [TestClass]
    public class ServicioRegistro
    {
        [TestMethod]
        public void RegistrarUsuario_RegistroExitoso_DeberiaRetornarTrue()
        {
            // Arrange
            //Pre-Condicion tiene que estar corriendo el servicio
            IServicioRegistro servicio = new ServicioRegistroClient();

            ServidorDescribeloPrueba.Usuario usuario = new ServidorDescribeloPrueba.Usuario()
            {
                Nombre = null
            };
            // Act
            var resultado = servicio.RegistrarUsuario(usuario);

            // Assert
            Assert.IsTrue(resultado, "El método no devolvió true para un registro exitoso.");
        }

    }
}
