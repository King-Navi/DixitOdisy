using System;
using System.Linq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioInvitacionPartida
    {
        public bool EnviarInvitacion(InvitacionPartida invitacion)
        {
            try
            {
                var jugador = jugadoresConectadosDiccionario.Values
                    .OfType<Usuario>()
                    .FirstOrDefault(busqueda => busqueda.Nombre.Equals(invitacion.NombreReceptor, StringComparison.OrdinalIgnoreCase));

                if (jugador == null)
                {
                    return false;
                }
                if (jugador.UsuarioSesionCallback is IUsuarioSesionCallback callback)
                {
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
    }
}
