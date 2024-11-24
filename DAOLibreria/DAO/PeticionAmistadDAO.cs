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
        public static bool GuardarSolicitudAmistad(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            if(idUsuarioRemitente == idUsuarioDestinatario) 
            { 
                return false; 
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var nuevaSolicitud = new PeticionAmistad
                    {
                        idRemitente = idUsuarioRemitente,
                        idDestinatario = idUsuarioDestinatario,
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
                    return context.PeticionAmistad.Any(fila =>
                        (fila.idRemitente == idRemitente && fila.idDestinatario == idDestinatario) ||
                        (fila.idRemitente == idDestinatario && fila.idDestinatario == idRemitente));
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
                        .Where(fila => fila.idDestinatario == idUsuario && fila.estado == ESTADO_SOLICITUD_PENDIENTE)
                        .ToList();

                    List<int> idsRemitentes = solicitudesPendientes
                        .Select(seleccion => seleccion.idRemitente)
                        .ToList();

                    List<Usuario> usuariosRemitentes = context.Usuario
                        .Where(fila => idsRemitentes.Contains(fila.idUsuario))
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
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var solicitud = context.PeticionAmistad
                                .FirstOrDefault(solicitudBuscada => 
                                    solicitudBuscada.idRemitente == idRemitente &&
                                    solicitudBuscada.idDestinatario == idDestinatario &&
                                    solicitudBuscada.estado == ESTADO_SOLICITUD_PENDIENTE);

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
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
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
