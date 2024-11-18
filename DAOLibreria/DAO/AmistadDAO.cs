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

        public static bool GuardarSolicitudAmistad(int idUsuario1, int idUsuario2)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    int idMayor = Math.Max(idUsuario1, idUsuario2);
                    int idMenor = Math.Min(idUsuario1, idUsuario2);

                    var nuevaSolicitud = new PeticionAmistad
                    {
                        id_Remitente = idMayor,       
                        id_Destinatario = idMenor,   
                        fechaPeticion = DateTime.Now, 
                        estado = "Pendiente"
                    };

                    context.PeticionAmistad.Add(nuevaSolicitud);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // TODO Manejar la excepción
                Console.WriteLine("Error al guardar la solicitud de amistad: " + ex.Message);
                throw;
            }
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

        public static bool ExisteSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            using (var context = new DescribeloEntities())
            {
                return context.PeticionAmistad.Any(p =>
                    (p.id_Remitente == idRemitente && p.id_Destinatario == idDestinatario) ||
                    (p.id_Remitente == idDestinatario && p.id_Destinatario == idRemitente));
            }
        }

        public static List<Usuario> ObtenerSolicitudesAmistad(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitudesPendientes = context.PeticionAmistad
                        .Where(s => s.id_Destinatario == idUsuario && s.estado == "Pendiente")
                        .ToList();

                    List<int> idsRemitentes = solicitudesPendientes
                        .Select(s => s.id_Remitente)
                        .ToList();

                    List<Usuario> usuariosRemitentes = context.Usuario
                        .Where(u => idsRemitentes.Contains(u.idUsuario))
                        .ToList();

                    return usuariosRemitentes;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en ObtenerSolicitudesAmistad: {ex.Message}");
                throw;
            }
        }

        public static bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.PeticionAmistad
                        .FirstOrDefault(s => s.id_Remitente == idRemitente && s.id_Destinatario == idDestinatario && s.estado == "Pendiente");

                    if (solicitud != null)
                    {
                        var nuevaAmistad = new Amigo
                        {
                            idMayor_usuario = Math.Max(idRemitente, idDestinatario),
                            idMenor_usuario = Math.Min(idRemitente, idDestinatario),
                            fechaInicioAmistad = DateTime.Now
                        };

                        context.Amigo.Add(nuevaAmistad);
                        context.PeticionAmistad.Remove(solicitud);

                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No se encontró la solicitud de amistad para aceptar.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al aceptar solicitud de amistad: {ex.Message}");
                throw;
            }
        }

        public static bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.PeticionAmistad
                        .FirstOrDefault(s => s.id_Remitente == idRemitente && s.id_Destinatario == idDestinatario && s.estado == "Pendiente");

                    if (solicitud != null)
                    {
                        context.PeticionAmistad.Remove(solicitud);

                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No se encontró la solicitud de amistad para rechazar.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al rechazar solicitud de amistad: {ex.Message}");
                throw;
            }
        }


    }
}

