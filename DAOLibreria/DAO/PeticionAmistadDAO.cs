using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.DAO
{
    public static class PeticionAmistadDAO
    {
        private const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";
        public static bool GuardarSolicitudAmistad(int idUsuario1, int idUsuario2)
        {
            if(idUsuario1 == idUsuario2) 
            { 
                return false; 
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var nuevaSolicitud = new PeticionAmistad
                    {
                        idRemitente = idUsuario1,
                        idDestinatario = idUsuario2,
                        fechaPeticion = DateTime.Now,
                        estado = ESTADO_SOLICITUD_PENDIENTE
                    };

                    context.PeticionAmistad.Add(nuevaSolicitud);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool ExisteSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    return context.PeticionAmistad.Any(p =>
                        (p.idRemitente == idRemitente && p.idDestinatario == idDestinatario) ||
                        (p.idRemitente == idDestinatario && p.idDestinatario == idRemitente));
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static List<Usuario> ObtenerSolicitudesAmistad(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitudesPendientes = context.PeticionAmistad
                        .Where(s => s.idDestinatario == idUsuario && s.estado == ESTADO_SOLICITUD_PENDIENTE)
                        .ToList();

                    List<int> idsRemitentes = solicitudesPendientes
                        .Select(s => s.idRemitente)
                        .ToList();

                    List<Usuario> usuariosRemitentes = context.Usuario
                        .Where(u => idsRemitentes.Contains(u.idUsuario))
                        .ToList();

                    return usuariosRemitentes;
                }
            }
            catch (Exception)
            {
                return new List<Usuario>();
            }
        }


        public static bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.PeticionAmistad
                        .FirstOrDefault(s => s.idRemitente == idRemitente && s.idDestinatario == idDestinatario && s.estado == ESTADO_SOLICITUD_PENDIENTE);

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
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.PeticionAmistad
                        .FirstOrDefault(s => s.idRemitente == idRemitente && s.idDestinatario == idDestinatario && s.estado == ESTADO_SOLICITUD_PENDIENTE);

                    if (solicitud != null)
                    {
                        context.PeticionAmistad.Remove(solicitud);

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
