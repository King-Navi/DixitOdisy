﻿using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.DAO.Utilidades;
using Pruebas.Servidor.Utilidades;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioIniciarSesion_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
        }
        [TestCleanup]

        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }

        #region ValidarCredenciales
        [TestMethod]
        public void ValidarCredenciales_CredencialesCorrectas_DeberiaRetornarUsuario()
        {
            
            imitarUsuarioDAO
            .Setup(dao => dao.ObtenerUsuarioPorNombre(It.IsAny<string>()))
            .Returns((string gamertag) =>
                {
                    // Simula el comportamiento del método
                    if (gamertag == "UsuarioExistente")
                    {
                        return new Usuario
                        {
                            idUsuario = 1,
                            gamertag = "UsuarioExistente",
                        };
                    }
                    return null; // Devuelve null si no encuentra el usuario
                });
            var nombreValido = "UsuarioExistente";
            var contraseniaValida = "contraseniaValida";
            
            var resultado = manejador.ValidarCredenciales(nombreValido, contraseniaValida);

            
            Assert.IsNotNull(resultado, "El método debería devolver un usuario válido.");
            Assert.AreEqual(nombreValido, resultado.Nombre, "El nombre del usuario debería coincidir.");
        }
        [TestMethod]
        public void ValidarCredenciales_CredencialesIncorrectas_DeberiaRetornarNulo()
        {
            
            string gamertagInvalido = "UsuarioInvalidoParaPruebas";
            string contraseniaInvalida = "ContraseniaIncorrecta123";

            
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }
        [TestMethod]
        public void ValidarCredenciales_ValorNulo_DeberiaRetornarNulo()
        {
            
            string gamertagInvalido = null;
            string contraseniaInvalida = null;

            
            var resultado = manejador.ValidarCredenciales(gamertagInvalido, contraseniaInvalida);

            
            Assert.IsNull(resultado, "El método debería devolver un nulo");
        }

        
        #endregion
    }
}
