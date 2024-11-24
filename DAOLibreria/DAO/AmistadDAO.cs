using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        .Where(filaAmigo => filaAmigo.idMayor_usuario == idUsuario || filaAmigo.idMenor_usuario == idUsuario)
                        .ToList();

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
            catch (Exception)
            {
            }
            return resultado;
        }


        public static bool SonAmigos(int idUsuario1, int idUsuario2)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    return context.Amigo.Any(fila =>
                        (fila.idMayor_usuario == idUsuario1 && fila.idMenor_usuario == idUsuario2) ||
                        (fila.idMayor_usuario == idUsuario2 && fila.idMenor_usuario == idUsuario1));
                }
            }
            catch (Exception)
            {
                return true;
            }
        }



    }
}

