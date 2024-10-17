using DAOLibreria.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
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

        public void AgregarAmigo(Usuario remitente, string destinatario) //FIXME
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
        private void EnviarSolicitudJugadorConectado(Usuario remitente, int idDestinatario) 
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



        public void AbrirCanalParaPeticiones(Usuario _usuario)
        {
            try
            {
                IServicioAmistadCallBack contexto = OperationContext.Current.GetCallbackChannel<IServicioAmistadCallBack>();
                List<DAOLibreria.ModeloBD.Usuario> usuarios = AmistadDAO.RecuperarListaAmigos(_usuario.IdUsuario);
                List<Amigo> amigos = new List<Amigo>();
                foreach (DAOLibreria.ModeloBD.Usuario usuario in usuarios)
                {
                    EstadoAmigo estadoJugador;
                    if (jugadoresConectadosDiccionario.ContainsKey(usuario.idUsuario))
                    {
                        AmigoConetado(_usuario, usuario);
                        estadoJugador = EstadoAmigo.Conectado;

                    }
                    else 
                    {
                        estadoJugador = EstadoAmigo.Desconectado;
                    }
                    amigos.Add(new Amigo()
                    {
                        Nombre = usuario.gamertag,
                        Estado = estadoJugador
                    });
                }
                contexto.ObtenerListaAmigoCallback(amigos);
            }
            catch (CommunicationException excepcion)
            { 
            
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.windows.weakeventmanager?view=windowsdesktop-8.0</ref>
        /// <param name="_remitente"></param>
        /// <param name="_destinatario"></param>
        //TODO: este metodo puede fallar al ser async, si un usuario se desconecta o los rescursos no estan disponibles puede haber problemas
        private async void AmigoConetado(Usuario _remitente, DAOLibreria.ModeloBD.Usuario _destinatario)
        {
            try
            {
                jugadoresConectadosDiccionario.TryGetValue(_remitente.IdUsuario, out UsuarioContexto remitente);
                jugadoresConectadosDiccionario.TryGetValue(_destinatario.idUsuario, out UsuarioContexto destinatario);

                lock (remitente)
                {
                    lock (destinatario)
                    {
                        //FIXME: No se me ocurrio otra idea mas que ocupar weakEventManager, se tiene que evaluar una mejor manera estar guardando evento/delegados puede no ser eficiente
                        //WeakEventManager<UsuarioContexto, EventArgs>.AddHandler(remitente, "DesconexionManejadorEvento", destinatario.ActualizarAmigo);
                        //WeakEventManager<UsuarioContexto, EventArgs>.AddHandler(destinatario, "DesconexionManejadorEvento", remitente.ActualizarAmigo);
                        EventHandler desconexionHandlerRemitente = null;
                        EventHandler desconexionHandlerDestinatario = null;
                        desconexionHandlerRemitente = (sender, e) =>
                        {
                            destinatario.ActualizarAmigo(sender, e);
                            remitente.DesconexionManejadorEvento -= desconexionHandlerRemitente;
                        };

                        desconexionHandlerDestinatario = (sender, e) =>
                        {
                            remitente.ActualizarAmigo(sender, e);
                            destinatario.DesconexionManejadorEvento -= desconexionHandlerDestinatario;
                        };
                        remitente.DesconexionManejadorEvento += desconexionHandlerRemitente;
                        destinatario.DesconexionManejadorEvento += desconexionHandlerDestinatario;
                    }
                }
            }
            catch (Exception)
            {

                
            }
        }

    }
}
