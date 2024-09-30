using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuarioSesion
    {
        /// <summary>
        /// Metodo que al iniciar sesion 
        /// </summary>
        /// <param name="idUsuario"></param>
        public void ObtenerSessionJugador(Usuario usuario)
        {
            try
            {
                IUsuarioSesionCallback contexto = OperationContext.Current.GetCallbackChannel<IUsuarioSesionCallback>();
                usuario.UsuarioSesionCallBack = contexto;
                bool sesionAbierta = jugadoresConetcadosDiccionario.ContainsKey(usuario.IdUsuario);
                if (!sesionAbierta)
                {
                    ICommunicationObject comunicacionObjecto = (ICommunicationObject)contexto;
                    usuario.CerrandoEvento = (emisor, e) =>
                    {
                        Console.WriteLine(usuario.Nombre + " se está yendo. Clase" + emisor);
                        comunicacionObjecto.Closing -= usuario.CerrandoEvento;
                    };
                    usuario.CerradoEvento= (emisor, e) =>
                    {
                        DesconectarJugador(usuario.IdUsuario);
                        Console.WriteLine(usuario.Nombre + " se ha ido. Clase" + emisor);
                        comunicacionObjecto.Closed -= usuario.CerradoEvento;
                    };
                    usuario.FalloEvento= (emisor, e) =>
                    {
                        DesconectarJugador(usuario.IdUsuario);
                        Console.WriteLine(usuario.Nombre + " ha fallado. Clase" + emisor);
                        comunicacionObjecto.Faulted -= usuario.FalloEvento;
                    };
                    // Suscribirse a los eventos
                    comunicacionObjecto.Closing += usuario.CerrandoEvento;
                    comunicacionObjecto.Closed += usuario.CerradoEvento;
                    comunicacionObjecto.Faulted += usuario.FalloEvento;
                    jugadoresConetcadosDiccionario.TryAdd(usuario.IdUsuario, usuario);

                }
                else
                {
                    UsuarioDuplicadoFalla fault = new UsuarioDuplicadoFalla()
                    { Motivo = "User '" + usuario.Nombre + "' already logged in!" };
                    throw new FaultException<UsuarioDuplicadoFalla>(fault);
                }
                contexto.ObtenerSessionJugadorCallback(sesionAbierta);

            }
            catch (CommunicationException ex)
            {
                //TODO: Manejar el error
            }
        }
    }
}
