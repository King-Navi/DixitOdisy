using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLibreria.DAO
{
    public static class PeticionAmistadDAO
    {
        private const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";
        public static bool GuardarSolicitudAmistad(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            if (idUsuarioRemitente == idUsuarioDestinatario)
            {
                return false;
            }
            if (SonAmigos(idUsuarioRemitente, idUsuarioDestinatario))
            {
                bool existeAmistad = true;
                bool existePeticion = false;
                throw new SolicitudAmistadExcepcion(existeAmistad, existePeticion);
            }

            if (ExisteSolicitudAmistad(idUsuarioRemitente, idUsuarioDestinatario))
            {
                bool existeAmistad = false;
                bool existePeticion = true;
                throw new SolicitudAmistadExcepcion(existeAmistad, existePeticion);
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al guardar la solicitud de amistad.", ex);
            }
        }

        private static bool SonAmigos(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            int idMayorUsuario = Math.Max(idUsuarioRemitente, idUsuarioDestinatario);
            int idMenorUsuario = Math.Min(idUsuarioRemitente, idUsuarioDestinatario);
            if (!(idUsuarioDestinatario > 0 || idUsuarioRemitente > 0))
            {
                return true;
            }
            try
            {
                return AmistadDAO.SonAmigos(idMayorUsuario, idMenorUsuario);
            }
            catch (Exception)
            {
            }
            return false;

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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al verificar la existencia de la solicitud de amistad.", ex);
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

                            if (solicitud == null)
                            {
                                return false;
                            }

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
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new InvalidOperationException("Error al aceptar la solicitud de amistad.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error en la operación de aceptación de solicitud de amistad.", ex);
            }
        }


        public static bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.PeticionAmistad
                        .FirstOrDefault(fila => fila.idRemitente == idRemitente && fila.idDestinatario == idDestinatario && fila.estado == ESTADO_SOLICITUD_PENDIENTE);

                    if (solicitud == null)
                    {
                        return false;
                    }

                    context.PeticionAmistad.Remove(solicitud);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al rechazar la solicitud de amistad.", ex);
            }
        }
    }
}
