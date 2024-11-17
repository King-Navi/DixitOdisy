using DAOLibreria.DAO;
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

        public void EnviarSolicitudAmistad(Usuario remitente, string destinatario) //FIXME
        {
            //TODO:Se necesita un metodo que convierta de gamertag a id de base de datos
            //ObtenerIdPorNombre(remitente.Nombre) En esta caso para prueba sera 2
            int idRemitente = 2;
            //ObtenerIdPorNombre(destinatario.Nombre) En esta caso para prueba sera 1
            int idDestinatario = 1;

            //TODO:Validar que no son amigos en la base de datos (Nota para la busqueda el primer id debe ser el alto)
            //ValidacionNoAmistad(id, id);
            if (false)
            {
                throw new FaultException<AmistadFalla>(new AmistadFalla(true, false));
            }

            var idMasAlto = Math.Max(idDestinatario, idRemitente);
            var idMasBajo = Math.Min(idDestinatario, idRemitente);
            //ExisteSolicitudAmistad(idMasAlto,idMasBajo)   Consulta a BD, colocar resultado en if
            if (false)
            {
                throw new FaultException<AmistadFalla>(new AmistadFalla(false, true));
            }

            if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario))
            {
                EnviarSolicitudJugadorConectado(remitente, idDestinatario);
            }
        }

        private void EnviarSolicitudJugadorConectado(Usuario remitente, int idDestinatario)  //FIXME
        {
            jugadoresConectadosDiccionario.TryGetValue(idDestinatario, out UsuarioContexto destinatarioConectado);
            lock (destinatarioConectado)
            {
                //((IUsuarioAmistad)destinatarioConectado).PeticionAmistadCallBack.ObtenerPeticionAmistadCallback(
                new SolicitudAmistad()
                {
                    Remitente = remitente
                };
            }
        }



        public  void AbrirCanalParaPeticiones(Usuario _usuarioRemitente)
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
                List<Usuario> amigosConetados= EnviarListaAmigos(_usuarioRemitente , contextoRemitente);
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

        private List<Usuario> EnviarListaAmigos(Usuario usuario, IAmistadCallBack contextoRemitente)
        {
            try
            {
                List<DAOLibreria.ModeloBD.Usuario> usuariosModeloBaseDatos = AmistadDAO.RecuperarListaAmigos(usuario.IdUsuario);
                List<Usuario> usuariosModeloWCF = new List<Usuario>();
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
                    Amigo amigo = new Amigo()
                    {
                        Nombre = amigoDestinario.gamertag,
                        Estado = estadoJugador,
                        Foto = new MemoryStream(amigoDestinario.fotoPerfil),
                        UltimaConexion = amigoDestinario.ultimaConexion.ToString()
                        
                    };
                    
                    contextoRemitente.ObtenerAmigoCallback(amigo);
                    if (amigo.Estado == EstadoAmigo.Conectado)
                    {
                        Usuario usuarioModeloWCF = new Usuario()
                        {
                            IdUsuario = amigoDestinario.idUsuario,
                            //FotoUsuario = new MemoryStream(amigoDestinario.fotoPerfil),
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
        private void AmigoConetado(Usuario _remitente, Usuario _destinatario)
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
                                    destinatario.AmigoDesconectado(remitente, new UsuarioDesconectadoEventArgs(((Usuario)remitente).Nombre, DateTime.Now));
                                    remitente.DesconexionManejadorEvento -= desconexionHandlerRemitente;
                                };

                                desconexionHandlerDestinatario = (sender, e) =>
                                {
                                    remitente.AmigoDesconectado(destinatario, new UsuarioDesconectadoEventArgs(((Usuario)destinatario).Nombre, DateTime.Now));
                                    destinatario.DesconexionManejadorEvento -= desconexionHandlerDestinatario;
                                };

                                remitente.DesconexionManejadorEvento += desconexionHandlerRemitente;
                                destinatario.DesconexionManejadorEvento += desconexionHandlerDestinatario;

                                remitente.EnviarAmigoActulizadoCallback(new Amigo(((Usuario)destinatario).Nombre, EstadoAmigo.Conectado));
                                destinatario.EnviarAmigoActulizadoCallback(new Amigo(((Usuario)remitente).Nombre, EstadoAmigo.Conectado));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {


            }
        }

    }
}
