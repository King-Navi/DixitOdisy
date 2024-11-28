using System;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuarioSesion
    {
        public void ObtenerSessionJugador(Usuario usuario)
        {
            if (usuario == null)
            {
               return;
            }
            bool sesionAbierta = jugadoresConectadosDiccionario.ContainsKey(usuario.IdUsuario);
            if (!sesionAbierta)
            {
                try
                {
                    IUsuarioSesionCallback contextoUsuario = contextoOperacion.GetCallbackChannel<IUsuarioSesionCallback>();
                    IAmistadCallBack contextoAmigos = contextoOperacion.GetCallbackChannel<IAmistadCallBack>();
                    IInvitacionPartidaCallback contextoInvitacionPartida = contextoOperacion.GetCallbackChannel<IInvitacionPartidaCallback>();
                    usuario.UsuarioSesionCallBack = contextoUsuario;
                    usuario.AmistadSesionCallBack = contextoAmigos;
                    usuario.InvitacionPartidaCallBack = contextoInvitacionPartida;
                    ICommunicationObject comunicacionObjecto = (ICommunicationObject)contextoUsuario;
                    usuario.CerrandoEvento = (emisor, e) =>
                    {
                        Console.WriteLine(usuario.Nombre + " | "+ usuario.IdUsuario + " se está yendo. Clase" + emisor);
                        comunicacionObjecto.Closing -= usuario.CerrandoEvento;
                    };
                    usuario.CerradoEvento = (emisor, e) =>
                    {
                        Console.WriteLine(usuario.Nombre + " | " + usuario.IdUsuario + " se ha ido. Clase" + emisor);
                        comunicacionObjecto.Closed -= usuario.CerradoEvento;
                        //Despues de este metodo el usario ya no existe porque se ocupa dispose
                        DesconectarUsuario(usuario.IdUsuario);
                    };
                    usuario.FalloEvento = (emisor, e) =>
                    {
                        Console.WriteLine(usuario.Nombre + " ha fallado. Clase" + emisor);
                        comunicacionObjecto.Faulted -= usuario.FalloEvento;
                        //Despues de este metodo el usario ya no existe porque se ocupa dispose
                        DesconectarUsuario(usuario.IdUsuario);
                    };
                    comunicacionObjecto.Closing += usuario.CerrandoEvento;
                    comunicacionObjecto.Closed += usuario.CerradoEvento;
                    comunicacionObjecto.Faulted += usuario.FalloEvento;
                    sesionAbierta = true;
                    if (ConectarUsuario(usuario))
                    {
                        contextoUsuario.ObtenerSessionJugadorCallback();
                    }
                }
                catch (CommunicationException)
                {
                    jugadoresConectadosDiccionario.TryRemove(usuario.IdUsuario, out _);
                    Console.WriteLine("Incio");
                }

            }
            else
            {
                UsuarioFalla excepcion = new UsuarioFalla()
                {
                    EstaConectado = true
                };
                throw new FaultException<UsuarioFalla>(excepcion, new FaultReason("El usuario ya está conectado"));
            }
        }

        public bool ConectarUsuario(Usuario usuario)
        {
            return jugadoresConectadosDiccionario.TryAdd(usuario.IdUsuario, usuario);
        }
    }
}

