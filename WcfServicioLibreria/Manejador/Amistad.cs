﻿using DAOLibreria.Excepciones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioAmistad
    {
        public bool EnviarSolicitudAmistad(Modelo.Usuario remitente, string destinatario)
        {
            try
            {
                int idRemitente = ObtenerIdPorNombre(remitente.Nombre);
                int idDestinatario = ObtenerIdPorNombre(destinatario);

                if (SonAmigos(idRemitente, idDestinatario))
                {
                    bool existeAmistad = true;
                    bool existePeticion = false;
                    throw new FaultException<SolicitudAmistadFalla>(
                        new SolicitudAmistadFalla(existeAmistad, existePeticion));
                }

                if (ExisteSolicitudAmistad(idRemitente, idDestinatario))
                {
                    bool existeAmistad = false;
                    bool existePeticion = true;
                    throw new FaultException<SolicitudAmistadFalla>(
                        new SolicitudAmistadFalla(existeAmistad, existePeticion));
                }

                if (GuardarSolicitudAmistad(idRemitente, idDestinatario))
                {
                    return true;
                }

                return false;
            }
            catch (SolicitudAmistadExcepcion excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw new FaultException<SolicitudAmistadFalla>(
                    new SolicitudAmistadFalla(excepcion.ExisteAmistad, excepcion.ExistePeticion));
            }
            catch (FaultException<SolicitudAmistadFalla> excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                throw new FaultException("Ocurrió un error inesperado al guardar la solicitud de amistad.");
            }
        }

        private bool GuardarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                return peticionAmistadDAO.GuardarSolicitudAmistad(idRemitente, idDestinatario);
            }
            catch (FaultException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }

            return false;

        }

        private int ObtenerIdPorNombre(string nombre)
        {
            int id = 0;
            try
            {
                id = usuarioDAO.ObtenerIdPorNombre(nombre);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return id;

        }

        public bool SonAmigos(string usuarioRemitente, string destinatario)
        {
            int idRemitente = ObtenerIdPorNombre(usuarioRemitente);
            int idDestinatario = ObtenerIdPorNombre(destinatario);
            int idMayorUsuario = Math.Max(idRemitente, idDestinatario);
            int idMenorUsuario = Math.Min(idRemitente, idDestinatario);
            if (!(idDestinatario > ID_INVALIDO && idRemitente > ID_INVALIDO))
            {
                return true;
            }
            return SonAmigos(idMayorUsuario, idMenorUsuario);
        }

        private bool SonAmigos(int idMasAlto, int idMasBajo)
        {
            try
            {
                return amistadDAO.SonAmigos(idMasAlto, idMasBajo);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;

        }

        private bool ExisteSolicitudAmistad(int idMasAlto, int idMasBajo)
        {
            try
            {
                return peticionAmistadDAO.ExisteSolicitudAmistad(idMasAlto, idMasBajo);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }

            return false;
        }

        public bool ConectarYBuscarAmigos(Modelo.Usuario usuarioRemitenteServidor)
        {
            try
            {
                jugadoresConectadosDiccionario.TryGetValue(usuarioRemitenteServidor.IdUsuario, out UsuarioContexto remitente);
                DAOLibreria.ModeloBD.Usuario usuarioRemitenteModeloBaseDatos = usuarioDAO.ObtenerUsuarioPorId(usuarioRemitenteServidor.IdUsuario);
                List<DAOLibreria.ModeloBD.Usuario> amigosConetados = EnviarListaAmigosAsync(usuarioRemitenteServidor, remitente.UsuarioSesionCallback);

                foreach (var usuarioDestino in amigosConetados)
                {
                    AmigoConetadoActualizar(usuarioRemitenteModeloBaseDatos, usuarioDestino);
                }
                return true;
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }

            return false;
        }

        private List<DAOLibreria.ModeloBD.Usuario> EnviarListaAmigosAsync(Modelo.Usuario usuario, IUsuarioSesionCallback contextoRemitente)
        {
            List<DAOLibreria.ModeloBD.Usuario> usuariosModeloWCF = new List<DAOLibreria.ModeloBD.Usuario>();

            try
            {
                IUsuarioSesionCallback contextoUsuarioSesion = contextoOperacion.GetCallbackChannel<IUsuarioSesionCallback>();

                List<DAOLibreria.ModeloBD.Usuario> usuariosModeloBaseDatos = amistadDAO.RecuperarListaAmigos(usuario.IdUsuario);
                foreach (DAOLibreria.ModeloBD.Usuario amigoDestinario in usuariosModeloBaseDatos)
                {
                    EstadoAmigo estadoJugador;

                    if (jugadoresConectadosDiccionario.ContainsKey(amigoDestinario.idUsuario))
                    {
                        estadoJugador = EstadoAmigo.Conectado;

                    }
                    else
                    {
                        estadoJugador = EstadoAmigo.Desconectado;
                    }
                    Modelo.Amigo amigo = new Modelo.Amigo()
                    {
                        Nombre = amigoDestinario.gamertag,
                        Estado = estadoJugador,
                        Foto = amigoDestinario.fotoPerfil,
                        UltimaConexion = amigoDestinario.ultimaConexion.ToString()

                    };
                    contextoUsuarioSesion?.ObtenerAmigoCallback(amigo);
                    if (amigo.Estado == EstadoAmigo.Conectado)
                    {
                        Modelo.Usuario usuarioModeloWCF = new Modelo.Usuario()
                        {
                            IdUsuario = amigoDestinario.idUsuario,
                            FotoUsuario = new MemoryStream(amigoDestinario.fotoPerfil),
                            Nombre = amigoDestinario.gamertag,
                            EstadoJugador = amigo.Estado == EstadoAmigo.Conectado ? EstadoUsuario.Conectado : EstadoUsuario.Desconectado
                        };
                    }
                }
                return usuariosModeloBaseDatos;

            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }

            return usuariosModeloWCF;
        }

        private void AmigoConetadoActualizar(DAOLibreria.ModeloBD.Usuario _remitente, DAOLibreria.ModeloBD.Usuario _destinatario)
        {
            try
            {
                if (jugadoresConectadosDiccionario.TryGetValue(_remitente.idUsuario, out UsuarioContexto remitente)
                    && jugadoresConectadosDiccionario.TryGetValue(_destinatario.idUsuario, out UsuarioContexto destinatario))
                {

                    EventHandler desconexionHandlerRemitente = null;
                    EventHandler desconexionHandlerDestinatario = null;
                    desconexionHandlerRemitente = (emisor, evento) =>
                    {
                        destinatario.AmigoDesconectado(_remitente, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)remitente).Nombre, DateTime.Now));
                        remitente.DesconexionEvento -= desconexionHandlerRemitente;
                    };

                    desconexionHandlerDestinatario = (emisor, evento) =>
                    {
                        remitente.AmigoDesconectado(_destinatario, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)destinatario).Nombre, DateTime.Now));
                        destinatario.DesconexionEvento -= desconexionHandlerDestinatario;
                    };

                    remitente.DesconexionEvento += desconexionHandlerRemitente;
                    destinatario.DesconexionEvento += desconexionHandlerDestinatario;

                    Task.Run(() => ActualizarAmigo(remitente, _destinatario, EstadoAmigo.Conectado));
                    Task.Run(() => ActualizarAmigo(destinatario, _remitente, EstadoAmigo.Conectado));
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        private void ActualizarAmigo(UsuarioContexto contexto, DAOLibreria.ModeloBD.Usuario amigo, EstadoAmigo estado)
        {
            try
            {
                contexto.EnviarAmigoActulizadoCallback(new Amigo()
                {
                    Foto = amigo.fotoPerfil,
                    Nombre = amigo.gamertag,
                    Estado = estado,
                    UltimaConexion = amigo.ultimaConexion.ToString()
                });
            }
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public List<SolicitudAmistad> ObtenerSolicitudesAmistad(Usuario usuario)
        {
            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosSolicitantes = peticionAmistadDAO.ObtenerSolicitudesAmistad(usuario.IdUsuario);
                List<SolicitudAmistad> usuariosModeloWCF = new List<SolicitudAmistad>();
                foreach (DAOLibreria.ModeloBD.Usuario usuarioBD in usuariosSolicitantes)
                {
                    Modelo.Usuario usuarioWCF = new Modelo.Usuario
                    {
                        IdUsuario = usuarioBD.idUsuario,
                        Nombre = usuarioBD.gamertag,
                        EstadoJugador = EstadoUsuario.Desconectado,
                        FotoUsuario = new MemoryStream(usuarioBD.fotoPerfil)
                    };
                    usuariosModeloWCF.Add(new SolicitudAmistad(usuarioWCF));
                }

                return usuariosModeloWCF;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return null;
        }

        public bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                EvaluarIdValido(idRemitente);
                EvaluarIdValido(idDestinatario);
                if (peticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario))
                {
                    if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario) &&
                        jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        var usuarioDestinoConectado = usuarioDAO.ObtenerUsuarioPorId(idDestinatario);
                        var usuarioRemitenteConectado = usuarioDAO.ObtenerUsuarioPorId(idRemitente);
                        AmigoConetadoActualizar(usuarioRemitenteConectado, usuarioDestinoConectado);
                        return true;
                    }
                    else if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idDestinatario, out UsuarioContexto destinatario);
                        var amigoRemitente = usuarioDAO.ObtenerUsuarioPorId(idRemitente);
                        Modelo.Amigo amigo = new Modelo.Amigo()
                        {
                            Nombre = amigoRemitente.gamertag,
                            Estado = EstadoAmigo.Desconectado,
                            Foto = amigoRemitente.fotoPerfil,
                            UltimaConexion = amigoRemitente.ultimaConexion.ToString()

                        };
                        destinatario.UsuarioSesionCallback?.ObtenerAmigoCallback(amigo);
                        return true;
                    }
                    else
                    {
                        ServidorFalla excepcion = new ServidorFalla()
                        {
                            EstaRemitenteDesconectado = true
                        };
                        throw new FaultException<ServidorFalla>(excepcion, new FaultReason("Te encuentras en estado invalido"));
                    }
                }
            }
            catch (FaultException<ServidorFalla>)
            {
                throw;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }

            return false;

        }

        public bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                EvaluarIdValido(idRemitente);
                EvaluarIdValido(idDestinatario);
                return peticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario);
            }
            catch (FaultException<ServidorFalla> excepcion)
            {
                throw excepcion;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public bool EliminarAmigo(string usuarioRemitenteNombre, string usuarioDestinatarioNombre)
        {

            int idRemitente = ObtenerIdPorNombre(usuarioRemitenteNombre);
            int idDestinatario = ObtenerIdPorNombre(usuarioDestinatarioNombre);
            int idMayorUsuario = Math.Max(idRemitente, idDestinatario);
            int idMenorUsuario = Math.Min(idRemitente, idDestinatario);
            try
            {
                EvaluarIdValido(idRemitente);
                EvaluarIdValido(idDestinatario);
                if (amistadDAO.EliminarAmigo(idMayorUsuario, idMenorUsuario))
                {
                    if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario) &&
                        jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto remitente);
                        jugadoresConectadosDiccionario.TryGetValue(idDestinatario, out UsuarioContexto destinatario);
                        remitente.UsuarioSesionCallback?.EliminarAmigoCallback(new Amigo()
                        {
                            Nombre = usuarioDestinatarioNombre
                        });
                        destinatario.UsuarioSesionCallback?.EliminarAmigoCallback(new Amigo()
                        {
                            Nombre = usuarioRemitenteNombre
                        });
                    }
                    else if (jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto remitente);
                        remitente.UsuarioSesionCallback?.EliminarAmigoCallback(new Amigo()
                        {
                            Nombre = usuarioDestinatarioNombre
                        });
                    }
                    else
                    {
                        ServidorFalla excepcion = new ServidorFalla()
                        {
                            EstaRemitenteDesconectado = true
                        };
                        throw new FaultException<ServidorFalla>(excepcion, new FaultReason("Te encuentras en estado invalido"));
                    }
                    return true;
                }
            }
            catch (FaultException<ServidorFalla> excepcion)
            {
                throw excepcion;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;

        }

        private void EvaluarIdValido(int identificador)
        {
            if (identificador <= ID_INVALIDO)
            {
                throw new ArgumentException();
            }
        }

    }


}

