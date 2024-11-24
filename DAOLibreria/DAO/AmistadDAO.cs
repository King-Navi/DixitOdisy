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

        public static bool SonAmigos(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    return context.Amigo.Any(fila =>
                        (fila.idMayor_usuario == idUsuarioRemitente && fila.idMenor_usuario == idUsuarioDestinatario) ||
                        (fila.idMayor_usuario == idUsuarioDestinatario && fila.idMenor_usuario == idUsuarioRemitente));
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool EliminarAmigo(int idUsuarioMayor, int idUsuarioMenor)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    int idMayor = Math.Max(idUsuarioMayor, idUsuarioMenor);
                    int idMenor = Math.Min(idUsuarioMayor, idUsuarioMenor);
                    var amistad = context.Amigo
                                         .SingleOrDefault(a => a.idMayor_usuario == idMayor && a.idMenor_usuario == idMenor);
                    if (amistad != null)
                    {
                        context.Amigo.Remove(amistad);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

