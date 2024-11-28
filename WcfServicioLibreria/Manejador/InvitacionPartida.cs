using System;
using System.Linq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioInvitacionPartida
    {
        public bool EnviarInvitacion(string gamertagEmisor, string codigoSala, string gamertagReceptor)
        {
            try
            {
                var jugador = jugadoresConectadosDiccionario.Values
                    .OfType<Usuario>()
                    .FirstOrDefault(busqueda => busqueda.Nombre.Equals(gamertagReceptor, StringComparison.OrdinalIgnoreCase));

                if (jugador == null)
                {
                    return false;
                }
                if (jugador.InvitacionPartidaCallBack is IInvitacionPartidaCallback callback)
                {
                    InvitacionPartida invitacion = new InvitacionPartida(gamertagEmisor, codigoSala, gamertagReceptor);
                    callback?.RecibirInvitacionCallback(invitacion);
                    return true;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;

        }

        public void AbrirCanalParaInvitaciones(Usuario usuarioRemitente)
        {
            try
            {
                IInvitacionPartidaCallback contextoRemitente = contextoOperacion.GetCallbackChannel<IInvitacionPartidaCallback>();
                if (jugadoresConectadosDiccionario.TryGetValue(usuarioRemitente.IdUsuario, out UsuarioContexto remitente))
                {
                    lock (remitente)
                    {
                        remitente.InvitacionPartidaCallBack = contextoRemitente;
                    }
                }
            }
            catch (Exception)
            {
                if (jugadoresConectadosDiccionario.TryGetValue(usuarioRemitente.IdUsuario, out UsuarioContexto remitente))
                {
                    lock (remitente)
                    {
                        remitente.InvitacionPartidaCallBack = null;
                    }
                }
            }
        }
    }
}
