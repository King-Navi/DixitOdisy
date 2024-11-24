using DAOLibreria.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioAmistad
    {
        public bool EnviarSolicitudAmistad(Modelo.Usuario remitente, string destinatario)
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
        private bool GuardarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            return PeticionAmistadDAO.GuardarSolicitudAmistad(idRemitente, idDestinatario);
        }
        private int ObtenerIdPorNombre(string nombre)
        {
            int id = 0;
            try
            {
                id = UsuarioDAO.ObtenerIdPorNombre(nombre);
            }
            catch (Exception)
            {
            }
            return id;

        }
        public bool SonAmigos(string usuarioRemitente, string destinatario)
        {
            int idRemitente = ObtenerIdPorNombre(usuarioRemitente);
            int idDestinatario = ObtenerIdPorNombre(destinatario);
            int idMayorUsuario = Math.Max(idRemitente, idDestinatario);
            int idMenorUsuario = Math.Min(idRemitente, idDestinatario);
            if (!(idDestinatario > 0 || idRemitente > 0))
            {
                return true;
            }
            return SonAmigos(idMayorUsuario, idMenorUsuario);

        }

        private bool SonAmigos(int idMasAlto, int idMasBajo)
        {
            try
            {
                return AmistadDAO.SonAmigos(idMasAlto, idMasBajo);
            }
            catch (Exception)
            {
            }
            return false;

        }

        private bool ExisteSolicitudAmistad(int idMasAlto, int idMasBajo)
        {
            try
            {
                return PeticionAmistadDAO.ExisteSolicitudAmistad(idMasAlto, idMasBajo);
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void AbrirCanalParaPeticiones(Modelo.Usuario _usuarioRemitente)
        {
            try
            {
                IAmistadCallBack contextoRemitente = contextoOperacion.GetCallbackChannel<IAmistadCallBack>();
                jugadoresConectadosDiccionario.TryGetValue(_usuarioRemitente.IdUsuario, out UsuarioContexto remitente);
                lock (remitente)
                {
                    remitente.AmistadSesionCallBack = contextoRemitente;
                }
                List<Modelo.Usuario> amigosConetados = EnviarListaAmigos(_usuarioRemitente, contextoRemitente);

                foreach (var usuarioDestino in amigosConetados)
                {
                    AmigoConetado(_usuarioRemitente, usuarioDestino);
                }
            }
            catch (Exception)
            {
            }
            return;
        }

        private List<Modelo.Usuario> EnviarListaAmigos(Modelo.Usuario usuario, IAmistadCallBack contextoRemitente)
        {
            List<Modelo.Usuario> usuariosModeloWCF = new List<Modelo.Usuario>();

            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosModeloBaseDatos = AmistadDAO.RecuperarListaAmigos(usuario.IdUsuario);
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
                        Foto = new MemoryStream(amigoDestinario.fotoPerfil),
                        UltimaConexion = amigoDestinario.ultimaConexion.ToString()

                    };
                    contextoRemitente.ObtenerAmigoCallback(amigo);
                    if (amigo.Estado == EstadoAmigo.Conectado)
                    {
                        Modelo.Usuario usuarioModeloWCF = new Modelo.Usuario()
                        {
                            IdUsuario = amigoDestinario.idUsuario,
                            FotoUsuario = new MemoryStream(amigoDestinario.fotoPerfil),
                            Nombre = amigoDestinario.gamertag,
                            EstadoJugador = amigo.Estado == EstadoAmigo.Conectado ? EstadoUsuario.Conectado : EstadoUsuario.Desconectado
                        };
                        usuariosModeloWCF.Add(usuarioModeloWCF);
                    }
                }
            }
            catch (Exception)
            {
            }
            return usuariosModeloWCF;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.windows.weakeventmanager?view=windowsdesktop-8.0</ref>
        /// <param name="_remitente"></param>
        /// <param name="_destinatario"></param>
        private void AmigoConetado(Modelo.Usuario _remitente, Modelo.Usuario _destinatario)
        {
            try
            {
                if (jugadoresConectadosDiccionario.TryGetValue(_remitente.IdUsuario, out UsuarioContexto remitente)
                    && jugadoresConectadosDiccionario.TryGetValue(_destinatario.IdUsuario, out UsuarioContexto destinatario))
                {
                    {
                        lock (remitente)
                        {
                            lock (destinatario)
                            {
                                EventHandler desconexionHandlerRemitente = null;
                                EventHandler desconexionHandlerDestinatario = null;
                                desconexionHandlerRemitente = (sender, e) =>
                                {
                                    destinatario.AmigoDesconectado(remitente, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)remitente).Nombre, DateTime.Now));
                                    remitente.DesconexionManejadorEvento -= desconexionHandlerRemitente;
                                };

                                desconexionHandlerDestinatario = (sender, e) =>
                                {
                                    remitente.AmigoDesconectado(destinatario, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)destinatario).Nombre, DateTime.Now));
                                    destinatario.DesconexionManejadorEvento -= desconexionHandlerDestinatario;
                                };

                                remitente.DesconexionManejadorEvento += desconexionHandlerRemitente;
                                destinatario.DesconexionManejadorEvento += desconexionHandlerDestinatario;

                                remitente.EnviarAmigoActulizadoCallback(new Amigo()
                                {
                                    Foto = ((Usuario)destinatario).FotoUsuario,
                                    Nombre = ((Usuario)destinatario).Nombre,
                                    Estado = EstadoAmigo.Conectado

                                });
                                destinatario.EnviarAmigoActulizadoCallback(new Modelo.Amigo(((Modelo.Usuario)remitente).Nombre, EstadoAmigo.Conectado));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //TODO MANEJAR LLA EXCEPCion

            }
        }

        public List<SolicitudAmistad> ObtenerSolicitudesAmistad(Modelo.Usuario usuario)
        {
            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosSolicitantes = PeticionAmistadDAO.ObtenerSolicitudesAmistad(usuario.IdUsuario);
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
            catch (Exception)
            {
            }
            return null;
        }

        public bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                return PeticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                return PeticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario); ;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
