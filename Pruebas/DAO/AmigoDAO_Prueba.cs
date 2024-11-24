using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOLibreria.DAO;
using Pruebas.DAO.Utilidades;

namespace Pruebas.DAO
{
    [TestClass]
    public class AmigoDAO_Prueba : ConfiguracionPruebaBD
    {
        private const int ID_USUARIO_MENOR = 1;
        private const int ID_USUARIO_MAYOR = 2;
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

        [TestMethod]
        public void EliminarAmigo_CuandoRelacionExiste_DeberiaRetornarTrue()
        {
            //Arrage
            ConfigurarAmistad(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            // Act
            bool resultado = AmistadDAO.EliminarAmigo(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            // Assert
            Assert.IsTrue(resultado, "El método debería devolver true cuando la relación de amistad existe y se elimina correctamente.");
        }
        [TestMethod]
        public void EliminarAmigo_CuandoRelacionNoExiste_DeberiaRetornarFalse()
        {
            // Arrange
            // ID que no tiene relación

            // Act
            bool resultado = AmistadDAO.EliminarAmigo(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando no existe una relación de amistad entre los usuarios.");
        }
        [TestMethod]
        public void EliminarAmigo_CuandoIdsInvalidos_DeberiaRetornarFalse()
        {
            // Arrange
            // Prencondcion IDs inválidos
            int idUsuarioA = -1; 
            int idUsuarioB = -2;

            // Act
            bool resultado = AmistadDAO.EliminarAmigo(idUsuarioA, idUsuarioB);

            // Assert
            Assert.IsFalse(resultado, "El método debería devolver false cuando los IDs proporcionados son inválidos.");
        }

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
    }
}
