using DAOLibreria.ModeloBD;
using DAOLibreria.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Pruebas.DAO.Utilidades;
using System.Diagnostics;

namespace Pruebas.DAO
{
    [TestClass]
    public class AmigoDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int ID_USUARIO_MENOR = 1;
        private const int ID_USUARIO_MAYOR = 2;
        private const int ID_INVALIDO_SEGUNDO_EJEMPLO = -2;
        private AmistadDAO amistadDAO = new AmistadDAO();

        [DebuggerStepThrough]
        private static void ConfigurarAmistad(int idMayor, int idMenor)
        {
            using (var context = new DescribeloEntities())
            {
                var amistad = new Amigo
                {
                    idMayor_usuario = Math.Max(idMayor, idMenor),
                    idMenor_usuario = Math.Min(idMayor, idMenor)
                };
                context.Amigo.Add(amistad);
                context.SaveChanges();
            }
        }

        [DebuggerStepThrough]
        [TestCleanup]
        public void LimpiarRegistrosDePrueba()
        {
            using (var context = new DescribeloEntities())
            {
                var amistades = context.Amigo
                    .Where(amistad => (amistad.idMayor_usuario == ID_USUARIO_MENOR || amistad.idMenor_usuario == ID_USUARIO_MENOR) ||
                                                   (amistad.idMayor_usuario == ID_USUARIO_MAYOR || amistad.idMenor_usuario == ID_USUARIO_MAYOR))
                    .ToList();
                context.Amigo.RemoveRange(amistades);
                context.SaveChanges();
            }
        }
        #region MyRegion


        [TestMethod]
        public void EliminarAmigo_CuandoRelacionExiste_DeberiaRetornarTrue()
        {
            ConfigurarAmistad(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            
            bool resultado = amistadDAO.EliminarAmigo(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando la relación de amistad existe y se elimina correctamente.");
        }
        [TestMethod]
        public void EliminarAmigo_CuandoRelacionNoExiste_DeberiaRetornarFalse()
        {
            

            
            bool resultado = amistadDAO.EliminarAmigo(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando no existe una relación de amistad entre los usuarios.");
        }
        [TestMethod]
        public void EliminarAmigo_CuandoIdsInvalidos_DeberiaRetornarFalse()
        {
            
            
            bool resultado = amistadDAO.EliminarAmigo(ID_INVALIDO, ID_INVALIDO_SEGUNDO_EJEMPLO);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando los IDs proporcionados son inválidos.");
        }
        #endregion

        #region RecuperarListaAmigos

        [TestMethod]
        public void RecuperarListaAmigos_CuandoUsuarioTieneAmigos_DeberiaRetornarListaAmigos()
        {
            
            ConfigurarAmistad(7, ID_USUARIO_MENOR);
            ConfigurarAmistad(6, ID_USUARIO_MENOR);
            
            var resultado = amistadDAO.RecuperarListaAmigos(ID_USUARIO_MENOR);

            
            Assert.IsNotNull(resultado, "El resultado no debería ser nulo.");
            Assert.AreEqual(2, resultado.Count, "El usuario debería tener 2 amigos.");
        }

        [TestMethod]
        public void RecuperarListaAmigos_CuandoUsuarioNoTieneAmigos_DeberiaRetornarListaVacia()
        {
            

            
            var resultado = amistadDAO.RecuperarListaAmigos(ID_INVALIDO);

            
            Assert.IsNotNull(resultado, "El resultado no debería ser nulo.");
            Assert.AreEqual(0, resultado.Count, "El usuario no debería tener amigos asociados.");
        }

        [TestMethod]
        public void RecuperarListaAmigos_CuandoUsuarioNoExiste_DeberiaRetornarListaVacia()
        {
            

            
            var resultado = amistadDAO.RecuperarListaAmigos(ID_INEXISTENTE);

            
            Assert.AreEqual(0, resultado.Count, "El resultado debería ser lista vacia cuando el usuario no existe.");
        }

        #endregion RecuperarListaAmigos
        #region SonAmigos

        [TestMethod]
        public void SonAmigos_CuandoUsuariosSonAmigos_DeberiaRetornarTrue()
        {
            
            ConfigurarAmistad(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);
            
            bool resultado = amistadDAO.SonAmigos(ID_USUARIO_MENOR, ID_USUARIO_MAYOR);

            
            Assert.IsTrue(resultado, "El método debería devolver true cuando los usuarios son amigos.");
        }
        [TestMethod]
        public void SonAmigos_CuandoUsuariosNoSonAmigos_DeberiaRetornarFalse()
        {
            
            
            bool resultado = amistadDAO.SonAmigos(ID_USUARIO_MENOR, ID_USUARIO_MAYOR);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando los usuarios no son amigos.");
        }

        [TestMethod]
        public void SonAmigos_CuandoUnoDeLosUsuariosNoExiste_DeberiaRetornarFalse()
        {
            

            
            bool resultado = amistadDAO.SonAmigos(ID_USUARIO_MAYOR, ID_INEXISTENTE);

            
            Assert.IsFalse(resultado, "El método debería devolver false cuando uno de los usuarios no existe.");
        }


        #endregion

    }
}