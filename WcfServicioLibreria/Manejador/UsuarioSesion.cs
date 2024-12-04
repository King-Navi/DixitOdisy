using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuarioSesion
    {
        public bool ObtenerSesionJugador(Usuario usuario)
        {
            if (usuario == null || usuario.IdUsuario <= 0 || usuario.Nombre == null)
            {
               return false;
            }
            bool sesionAbierta = jugadoresConectadosDiccionario.ContainsKey(usuario.IdUsuario);
            if (!sesionAbierta)
            {
                try
                {
                    IUsuarioSesionCallback contextoUsuario = contextoOperacion.GetCallbackChannel<IUsuarioSesionCallback>();
                    Usuario nuevoUsuario = new Usuario(usuario.IdUsuario, usuario.Nombre , contextoUsuario);
                    nuevoUsuario.DesconexionManejador = 
                        new DesconectorEventoManejador( (ICommunicationObject)contextoUsuario, nuevoUsuario, nuevoUsuario.Nombre);
                    ((UsuarioContexto)nuevoUsuario).DesconexionEvento += DesconectarUsuario;
                    sesionAbierta = true;
                    if (ConectarUsuario(nuevoUsuario))
                    {
                        contextoUsuario.ObtenerSesionJugadorCallback();
                        return true;
                    }
                }
                catch (CommunicationException excepcion)
                {
                    jugadoresConectadosDiccionario.TryRemove(usuario.IdUsuario, out _);
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
            return false;
        }

        public bool ConectarUsuario(Usuario usuario)
        {
            return jugadoresConectadosDiccionario.TryAdd(usuario.IdUsuario, usuario);
        }
    }
}

