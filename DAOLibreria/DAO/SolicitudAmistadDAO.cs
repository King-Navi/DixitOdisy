using DAOLibreria.Excepciones;
using DAOLibreria.Interfaces;
using DAOLibreria.ModeloBD;
using DAOLibreria.Utilidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DAOLibreria.DAO
{
    public class SolicitudAmistadDAO : ISolicitudAmistadDAO
    {
        private const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";
        public bool GuardarSolicitudAmistad(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            if (idUsuarioRemitente == idUsuarioDestinatario)
            {
                return false;
            }
            var amigos = new AmistadDAO();
            if (amigos.SonAmigos(idUsuarioRemitente, idUsuarioDestinatario))
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
                    var nuevaSolicitud = new SolicitudAmistad
                    {
                        idRemitente = idUsuarioRemitente,
                        idDestinatario = idUsuarioDestinatario,
                        fechaSolicitud = DateTime.Now,
                        estado = ESTADO_SOLICITUD_PENDIENTE
                    };

                    context.SolicitudAmistad.Add(nuevaSolicitud);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }

        public bool ExisteSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    return context.SolicitudAmistad.Any(fila =>
                        (fila.idRemitente == idRemitente && fila.idDestinatario == idDestinatario) ||
                        (fila.idRemitente == idDestinatario && fila.idDestinatario == idRemitente));
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return true;
        }

        public List<Usuario> ObtenerSolicitudesAmistad(int idUsuario)
        {
            List<Usuario> usuariosRemitentes = new List<Usuario>();
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitudesPendientes = context.SolicitudAmistad
                        .Where(fila => fila.idDestinatario == idUsuario && fila.estado == ESTADO_SOLICITUD_PENDIENTE)
                        .ToList();

                    List<int> idsRemitentes = solicitudesPendientes
                        .Select(seleccion => seleccion.idRemitente)
                        .ToList();

                    usuariosRemitentes = context.Usuario
                        .Where(fila => idsRemitentes.Contains(fila.idUsuario))
                        .ToList();

                    return usuariosRemitentes;
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return usuariosRemitentes;
        }

        public bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var solicitud = context.SolicitudAmistad
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
                            context.SolicitudAmistad.Remove(solicitud);
                            context.SaveChanges();
                            transaction.Commit();
                            return true;
                        }
                        catch (ArgumentNullException excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarFatalException(excepcion);
                            return false;
                        }
                    }
                }
            }
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }

        public bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var solicitud = context.SolicitudAmistad
                        .FirstOrDefault(fila => fila.idRemitente == idRemitente && fila.idDestinatario == idDestinatario && fila.estado == ESTADO_SOLICITUD_PENDIENTE);

                    if (solicitud == null)
                    {
                        return false;
                    }

                    context.SolicitudAmistad.Remove(solicitud);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }
    }
}
