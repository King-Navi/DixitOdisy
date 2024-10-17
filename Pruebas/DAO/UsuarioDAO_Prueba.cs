using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilidadesLibreria;

namespace Pruebas.DAO
{
    [TestClass]
    public class UsuarioDAO_Prueba
    {
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsCoinciden_DeberiaRegistrarCorrectamente()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no esta configurada.");
            }

            var usuario = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = "Jugador123",
                fotoPerfil = File.ReadAllBytes("C:\\Users\\USER\\Downloads\\profile.jpg")
            };

            var usuarioCuenta = new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = "Jugador123",
                hashContrasenia = "d4735e3a265e16eee03f59718b9b5d03019c07d8b6c51f90da3a666eec13ab35",
                correo = "jugador123@ejemplo.com"
            };

            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);

            // Assert
            Assert.IsTrue(resultadoPrueba, "El registro debería haber sido exitoso porque los gamertags coinciden.");
        }
    }
}
