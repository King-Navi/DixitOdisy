using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.DAO
{
    public class AmistadDAO
    {
        public static List<Usuario> RecuperarListaAmigos(int idUsuario)
        {
            List<Usuario> resultado = null;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    List<Amigo> consultaAmigo = context.Amigo
                        .Where(filaAmigo => filaAmigo.idMayor_usuario == idUsuario 
                                                    || filaAmigo.idMenor_usuario == idUsuario).ToList();

                    List<int> idsUsuarios = consultaAmigo
                        .Select(filaAmigo => filaAmigo.idMayor_usuario)
                        .Union(consultaAmigo.Select(filaAmigo => filaAmigo.idMenor_usuario))
                        .ToList();
                    idsUsuarios.Remove(idUsuario);

                    List<Usuario> consultaUsuario = context.Usuario
                        .Where(filaUsuario => idsUsuarios.Contains(filaUsuario.idUsuario))
                        .ToList();

                    resultado = consultaUsuario;
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
                Console.WriteLine(excepcion);
                Console.WriteLine(excepcion.StackTrace);
                throw;
            }
            return resultado;
        }
        public static bool SonAmigos(int idUsuario1, int idUsuario2)
        {
            using (var context = new DescribeloEntities())
            {
                return context.Amigo.Any(a =>
                    (a.idMayor_usuario == idUsuario1 && a.idMenor_usuario == idUsuario2) ||
                    (a.idMayor_usuario == idUsuario2 && a.idMenor_usuario == idUsuario1));
            }
        }
    }
}
