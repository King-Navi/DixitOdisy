﻿using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        #region RegistrarNuevoUsuario
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsCoinciden_DeberiaRegistrar()
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
            var usuario = Utilidad.GenerarUsuarioDePrueba();
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba(usuario.gamertag);
            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            // Assert
            Assert.IsTrue(resultadoPrueba, "El registro debería haber sido exitoso porque los gamertags coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoLosGamertagsNoCoinciden_DeberiaFallar()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            var usuario = Utilidad.GenerarUsuarioDePrueba();
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);
            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
            // Assert
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque los gamertags no coinciden.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioEsNulo_DeberiaFallar()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }

            Usuario usuario = null;
            var numeroAleatorio = new Random((int)DateTime.Now.Ticks);
            var usuarioCuenta = Utilidad.GenerarUsuarioCuentaDePrueba("JugadorPrueba" + numeroAleatorio);

            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);

            // Assert
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque Usuario es nulo.");
        }
        [TestMethod]
        public void RegistrarNuevoUsuario_CuandoUsuarioYaExiste_DeberiaFallar()
        {
            // Arrange
            //Pre-condicion: El usuario debe estar ya registrado en base de datos
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
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
            // Act
            bool resultadoPrueba = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(nuevoUsuario, nuevoUsuarioCuenta);
            // Assert
            Assert.IsFalse(resultadoPrueba, "El registro no debería ser exitoso porque el usuario ya existe en la base de datos.");
        }
        #endregion

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CuandoCredencialesSonValidas_DeberiaRetornarUsuario()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            //Debe ser un UsuarioCuenta existente en base de datos
            string gamertagValido = "unaay";
            string contraseniaValida = "b7a88e8d61d649a44848a48c8de0e6bd48d2fd4d7a61cb733301634d5eac5080";
            // Act
            Usuario usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagValido, contraseniaValida);
            // Assert
            Assert.IsNotNull(usuario, "El usuario debería ser retornado cuando las credenciales son válidas.");
            Assert.AreEqual(gamertagValido, usuario.gamertag, "El gamertag del usuario retornado debería coincidir con el gamertag proporcionado.");
        }

        [TestMethod]
        public void ValidarCredenciales_CuandoContraseniaEsIncorrecta_DeberiaRetornarNull()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }

            //Debe ser un UsuarioCuenta NO existente en base de datos
            string gamertagValido = "NaviKing";
            string contraseniaInvalida = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4a";

            // Act
            Usuario usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagValido, contraseniaInvalida);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando la contraseña es incorrecta.");
        }
        [TestMethod]
        public void ValidarCredenciales_CuandoEsNulo_DeberiaRetornarNull()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }

            string gamertagInvalido= null;
            string contrasenia = null; // No importa el hash porque el gamertag no existe.

            // Act
            Usuario usuario = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertagInvalido, contrasenia);

            // Assert
            Assert.IsNull(usuario, "No se debería retornar un usuario cuando el gamertag no existe en la base de datos.");
        }
        #endregion

        #region GetUsuarioById
        [TestMethod]
        public void GetUsuarioById_CuandoIdExiste_DeberiaRetornarUsuario()
        {
            // Arrange
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }

            // Supongamos que el ID 1 corresponde a un usuario existente en la base de datos.
            int idExistente = 1;

            // Act
            Usuario usuario = DAOLibreria.DAO.UsuarioDAO.GetUsuarioById(idExistente);

            // Assert
            Assert.IsNotNull(usuario, "El método debería retornar un usuario cuando el ID existe.");
            Assert.AreEqual(idExistente, usuario.idUsuario, "El ID del usuario retornado debería coincidir con el ID proporcionado.");
        }
        #endregion
    }
}
