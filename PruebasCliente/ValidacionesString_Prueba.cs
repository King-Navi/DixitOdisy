using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System;
using WpfCliente.Utilidad;

namespace PruebasCliente
{
    [TestClass]
    public class ValidacionesString_Prueba

    {
        [TestMethod]
        public void EsGamertagValido_Correcto_ReturnsTrue()
        {
            string gamertag = "Player123";
            bool resultado = ValidacionesString.EsGamertagValido(gamertag);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void EsGamertagValido_ContieneEspacios_ReturnsFalse()
        {
            string gamertag = "Player 123";
            bool resultado = ValidacionesString.EsGamertagValido(gamertag);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsGamertagValido_ContienePalabraGuest_ReturnsFalse()
        {
            string gamertag = "GuestPlayer";
            bool resultado = ValidacionesString.EsGamertagValido(gamertag);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsGamertagValido_Timeout_ReturnsFalse()
        {
            string gamertag = new string('a', 100000);
            bool resultado = ValidacionesString.EsGamertagValido(gamertag);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsCorreoValido_Correcto_ReturnsTrue()
        {
            string correo = "usuario@gmail.com";
            bool resultado = ValidacionesString.EsCorreoValido(correo);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void EsCorreoValido_ConEspacios_ReturnsFalse()
        {
            string correo = "usuario @mail.com";
            bool resultado = ValidacionesString.EsCorreoValido(correo);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsCorreoValido_FormatoIncorrecto_ReturnsFalse()
        {
            string correo = "usuario.mail.com";
            bool resultado = ValidacionesString.EsCorreoValido(correo);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsCorreoValido_Timeout_ReturnsFalse()
        {
            string correo = new string('a', 90) + "@example.com";
            bool resultado = ValidacionesString.EsCorreoValido(correo);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsSimboloValido_ContieneSimbolos_ReturnsTrue()
        {
            string password = "P@ssw0rd!";
            bool resultado = ValidacionesString.EsSimboloValido(password);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void EsSimboloValido_SinSimbolos_ReturnsFalse()
        {
            string password = "Password123";
            bool resultado = ValidacionesString.EsSimboloValido(password);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void EsSimboloValido_SoloSimbolos_ReturnsTrue()
        {
            string password = "@#$%^&*";
            bool resultado = ValidacionesString.EsSimboloValido(password);
            Assert.IsTrue(resultado);
        }
    }
}
