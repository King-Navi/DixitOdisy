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

            // Generar un gamertag aleatorio
            string gamertagAleatorio = "Jugador" + new Random().Next(1000, 9999);

            // Generar una contraseña aleatoria y su hash SHA256
            string contraseniaAleatoria = "Contraseña" + new Random().Next(1000, 9999);
            string hashContrasenia = Utilidad.ObtenerSHA256Hash(contraseniaAleatoria);

            // Generar un correo electrónico aleatorio
            string correoAleatorio = $"{gamertagAleatorio.ToLower()}@ejemplo.com";

            // Generar un arreglo de bytes aleatorio para la foto de perfil
            byte[] fotoPerfilAleatoria = Utilidad.GenerarBytesAleatorios(256); // 256 bytes aleatorios

            var usuario = new DAOLibreria.ModeloBD.Usuario
            {
                gamertag = gamertagAleatorio,
                fotoPerfil = fotoPerfilAleatoria
            };

            var usuarioCuenta = new DAOLibreria.ModeloBD.UsuarioCuenta
            {
                gamertag = gamertagAleatorio,
                hashContrasenia = hashContrasenia,
                correo = correoAleatorio
            };

            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);

            // Assert
            Assert.IsTrue(resultadoPrueba, "El registro debería haber sido exitoso porque los gamertags coinciden.");
        }
    }
}
