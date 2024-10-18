using System;
using System.Runtime.Remoting.Contexts;
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
                    IUsuarioSesionCallback contexto = contextoOperacion.GetCallbackChannel<IUsuarioSesionCallback>();
                    usuario.UsuarioSesionCallBack = contexto;
                    ICommunicationObject comunicacionObjecto = (ICommunicationObject)contexto;
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
                    contexto.ObtenerSessionJugadorCallback(sesionAbierta);
                    jugadoresConectadosDiccionario.TryAdd(usuario.IdUsuario, usuario);

                }
                catch (CommunicationException excepcion)
                {
                    //TODO: Manejar el error
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
    }
}

