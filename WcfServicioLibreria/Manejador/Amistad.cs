using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

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

            if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario))
            {
                EnviarSolicitudJugadorConectado(remitente, idDestinatario);
            }

            if (GuardarSolicitudAmistad(idRemitente, idDestinatario))
            {
                return true;
            }

            return false;
        }

        private bool GuardarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            return AmistadDAO.GuardarSolicitudAmistad(idRemitente, idDestinatario);
        }

        private int ObtenerIdPorNombre(string nombre)
        {
            int id = 0;
            try
            {
                id = UsuarioDAO.ObtenerIdPorNombre(nombre);
                return id;
            }
            catch (Exception)
            {
                //Manejar la excepcion
                throw;
            }
        }

        private bool SonAmigos(int idMasAlto, int idMasBajo)
        {
            try
            {
                return AmistadDAO.SonAmigos(idMasAlto, idMasBajo);
            }
            catch (Exception ex)
            {
                //TODO MANEJAR EL ERROR
                Console.WriteLine($"Error en ValidacionNoAmistad: {ex.Message}");
                throw;
            }
        }

        private bool ExisteSolicitudAmistad(int idMasAlto, int idMasBajo)
        {
            try
            {
                return AmistadDAO.ExisteSolicitudAmistad(idMasAlto, idMasBajo);
            }
            catch (Exception ex)
            {
                //TODO MANEJAR EL ERROR
                Console.WriteLine($"Error en ExisteSolicitudAmistad: {ex.Message}");
                throw;
            }
        }

        public bool EnviarSolicitudJugadorConectado(Modelo.Usuario remitente, int idDestinatario)
        {
            try
            {
                if (jugadoresConectadosDiccionario.TryGetValue(idDestinatario, out UsuarioContexto destinatarioConectado))
                {
                    var callback = destinatarioConectado.AmistadSesionCallBack as IAmistadCallBack;

                    if (callback != null)
                    {
                        SolicitudAmistad solicitud = new SolicitudAmistad
                        {
                            Remitente = remitente
                        };

                        callback.ObtenerPeticionAmistadCallback(solicitud);
                        Console.WriteLine($"Solicitud de amistad enviada a {destinatarioConectado.Nombre}");
                        return true;
                    }
                    else
                    {
                        //TODO MANEJAR EL ERROR
                        Console.Error.WriteLine("El callback del receptor no es válido.");
                        return false;
                    }
                }
                else
                {
                    //TODO MANEJAR EL ERROR
                    Console.Error.WriteLine("El usuario receptor no está conectado.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                //TODO MANEJAR EL ERROR
                Console.Error.WriteLine($"Error al enviar solicitud de amistad: {ex.Message}");
                return false;
            }
        }


        public  void AbrirCanalParaPeticiones(Modelo.Usuario _usuarioRemitente)
        {
            try
            {
                IAmistadCallBack contextoRemitente = contextoOperacion.GetCallbackChannel<IAmistadCallBack>();
                jugadoresConectadosDiccionario.TryGetValue(_usuarioRemitente.IdUsuario, out UsuarioContexto remitente);
                lock (remitente)
                {
                    remitente.AmistadSesionCallBack = contextoRemitente;
                }
                //TODO:Talvez esto se pueda dividir en otra llamada, para mejorar la lectura
                List<Modelo.Usuario> amigosConetados= EnviarListaAmigos(_usuarioRemitente , contextoRemitente);
                foreach (var usuarioDestino in amigosConetados)
                {
                    AmigoConetado(_usuarioRemitente, usuarioDestino);
                }
            }
            catch (Exception)
            {
                jugadoresConectadosDiccionario.TryGetValue(_usuarioRemitente.IdUsuario, out UsuarioContexto remitente);
                if (remitente != null)
                lock (remitente)
                {
                    remitente.AmistadSesionCallBack = null;
                }
                throw;
            }
            return;
        }

        private List<Modelo.Usuario> EnviarListaAmigos(Modelo.Usuario usuario, IAmistadCallBack contextoRemitente)
        {
            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosModeloBaseDatos = AmistadDAO.RecuperarListaAmigos(usuario.IdUsuario);
                List<Modelo.Usuario> usuariosModeloWCF = new List<Modelo.Usuario>();
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
                        Foto = new MemoryStream(amigoDestinario.fotoPerfil)
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

                return usuariosModeloWCF;
            }
            catch (Exception excepcion)
            {
                Console.Error.WriteLine(excepcion.Message);
                Console.WriteLine(excepcion.StackTrace);
                throw;
            }
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

                                remitente.EnviarAmigoActulizadoCallback(new Modelo.Amigo(((Modelo.Usuario)destinatario).Nombre, EstadoAmigo.Conectado));
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

        public List<Modelo.Usuario> ObtenerSolicitudesAmistad(Modelo.Usuario usuario)
        {
            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosSolicitantes = AmistadDAO.ObtenerSolicitudesAmistad(usuario.IdUsuario);
                List<Modelo.Usuario> usuariosModeloWCF = new List<Modelo.Usuario>();
                foreach (DAOLibreria.ModeloBD.Usuario usuarioBD in usuariosSolicitantes)
                {
                    Modelo.Usuario usuarioWCF = new Modelo.Usuario
                    {
                        IdUsuario = usuarioBD.idUsuario,
                        Nombre = usuarioBD.gamertag,
                        EstadoJugador = EstadoUsuario.Desconectado, 
                        FotoUsuario = new MemoryStream(usuarioBD.fotoPerfil)
                    };

                    usuariosModeloWCF.Add(usuarioWCF);
                }

                return usuariosModeloWCF;
            }
            catch (Exception ex)
            {
                //TODO
                Console.Error.WriteLine($"Error en ObtenerSolicitudesAmistad: {ex.Message}");
                throw;
            }
        }

        public bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try
            {
                return AmistadDAO.AceptarSolicitudAmistad(idRemitente, idDestinatario);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en AceptarSolicitudAmistad: {ex.Message}");
                throw;
            }
        }


        public bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario)
        {
            try 
            { 
                return AmistadDAO.RechazarSolicitudAmistad(idRemitente, idDestinatario); ;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en RechazarSolicitudAmistad: {ex.Message}");
                throw;
            }
        }


    }
}
