using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using System;
using System.Collections.Generic;
using System.IO;
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

                if(idRemitente == 0 || idDestinatario == 0)
                {   
                    return false;
                }

                if (GuardarSolicitudAmistad(idRemitente, idDestinatario))
                {
                    return true;
                }

                return false;
            }
            catch (SolicitudAmistadExcepcion ex)
            {
                throw new FaultException<SolicitudAmistadFalla>(
                    new SolicitudAmistadFalla(ex.ExisteAmistad, ex.ExistePeticion));
            }
            catch (Exception ex)
            {
                throw new FaultException("Ocurrió un error inesperado al enviar la solicitud de amistad.");
            }
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

        public bool AbrirCanalParaPeticiones(Modelo.Usuario _usuarioRemitente)
        {
            try
            {
                IAmistadCallBack contextoRemitente = contextoOperacion.GetCallbackChannel<IAmistadCallBack>();
                jugadoresConectadosDiccionario.TryGetValue(_usuarioRemitente.IdUsuario, out UsuarioContexto remitente);
                lock (remitente)
                {
                    remitente.AmistadSesionCallBack = contextoRemitente;
                }
                DAOLibreria.ModeloBD.Usuario usuarioRemitenteModeloBaseDatos = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorId(_usuarioRemitente.IdUsuario);
                List<DAOLibreria.ModeloBD.Usuario> amigosConetados = EnviarListaAmigosAsync(_usuarioRemitente, contextoRemitente);

                foreach (var usuarioDestino in amigosConetados)
                {
                    AmigoConetado(usuarioRemitenteModeloBaseDatos, usuarioDestino);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private List<DAOLibreria.ModeloBD.Usuario> EnviarListaAmigosAsync(Modelo.Usuario usuario, IAmistadCallBack contextoRemitente)
        {
            List<DAOLibreria.ModeloBD.Usuario> usuariosModeloWCF = new List<DAOLibreria.ModeloBD.Usuario>();

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
                    contextoRemitente?.ObtenerAmigoCallback(amigo);
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
            catch (Exception)
            {
            }
            return usuariosModeloWCF;
        }

        private void AmigoConetado(DAOLibreria.ModeloBD.Usuario _remitente, DAOLibreria.ModeloBD.Usuario _destinatario)
        {
            try
            {
                if (jugadoresConectadosDiccionario.TryGetValue(_remitente.idUsuario, out UsuarioContexto remitente)
                    && jugadoresConectadosDiccionario.TryGetValue(_destinatario.idUsuario, out UsuarioContexto destinatario))
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
                                    destinatario.AmigoDesconectado(_remitente, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)remitente).Nombre, DateTime.Now));
                                    remitente.DesconexionManejadorEvento -= desconexionHandlerRemitente;
                                };

                                desconexionHandlerDestinatario = (sender, e) =>
                                {
                                    remitente.AmigoDesconectado(_destinatario, new UsuarioDesconectadoEventArgs(((Modelo.Usuario)destinatario).Nombre, DateTime.Now));
                                    destinatario.DesconexionManejadorEvento -= desconexionHandlerDestinatario;
                                };

                                remitente.DesconexionManejadorEvento += desconexionHandlerRemitente;
                                destinatario.DesconexionManejadorEvento += desconexionHandlerDestinatario;

                                remitente.EnviarAmigoActulizadoCallback(new Amigo()
                                {
                                    Foto = new MemoryStream(_destinatario.fotoPerfil),
                                    Nombre = _destinatario.gamertag,
                                    Estado = EstadoAmigo.Conectado,
                                    UltimaConexion = _destinatario.ultimaConexion.ToString()

                                });
                                destinatario.EnviarAmigoActulizadoCallback(new Amigo()
                                {
                                    Foto = new MemoryStream(_remitente.fotoPerfil),
                                    Nombre = _remitente.gamertag,
                                    Estado = EstadoAmigo.Conectado,
                                    UltimaConexion = _remitente.ultimaConexion.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public List<SolicitudAmistad> ObtenerSolicitudesAmistad(Usuario usuario)
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
                EvaluarIdValido(idRemitente);
                EvaluarIdValido(idDestinatario);
                if (PeticionAmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario))
                {
                    if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario) &&
                        jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        var usuarioDestinoConectado = UsuarioDAO.ObtenerUsuarioPorId(idDestinatario);
                        var usuarioRemitenteConectado = UsuarioDAO.ObtenerUsuarioPorId(idRemitente);
                        AmigoConetado(usuarioRemitenteConectado, usuarioDestinoConectado);
                    }
                    else if (jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto remitente);
                        var amigoDestinario = UsuarioDAO.ObtenerUsuarioPorId(idDestinatario);
                        Modelo.Amigo amigo = new Modelo.Amigo()
                        {
                            Nombre = amigoDestinario.gamertag,
                            Estado = EstadoAmigo.Desconectado,
                            Foto = new MemoryStream(amigoDestinario.fotoPerfil),
                            UltimaConexion = amigoDestinario.ultimaConexion.ToString()

                        };
                        remitente.AmistadSesionCallBack?.ObtenerAmigoCallback(amigo);
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
            catch (Exception)
            {
            }
            return false;
        }


        public bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                EvaluarIdValido(idRemitente);
                EvaluarIdValido(idDestinatario);
                return PeticionAmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario); ;
            }
            catch (Exception)
            {
                return false;
            }
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
                if (AmistadDAO.EliminarAmigo(idMayorUsuario, idMenorUsuario))
                {
                    if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario) && 
                        jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto remitente);
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto destinatario);
                        remitente.AmistadSesionCallBack?.EliminarAmigoCallback(new Amigo()
                        {
                            Nombre = usuarioDestinatarioNombre
                        });
                        destinatario.AmistadSesionCallBack?.EliminarAmigoCallback(new Amigo()
                        {
                            Nombre = usuarioRemitenteNombre
                        });
                    }
                    else if (jugadoresConectadosDiccionario.ContainsKey(idRemitente))
                    {
                        jugadoresConectadosDiccionario.TryGetValue(idRemitente, out UsuarioContexto remitente);
                        remitente.AmistadSesionCallBack?.EliminarAmigoCallback(new Amigo()
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
            catch(FaultException<ServidorFalla> excepcion)
            {
                throw excepcion;
            }
            catch (Exception)
            {
            }
            return false;

        }

        private void EvaluarIdValido(int identificador)
        {
            if (identificador <= 0 || identificador <= 0)
            {
                throw new ArgumentException();
            }
        }
    }
}
