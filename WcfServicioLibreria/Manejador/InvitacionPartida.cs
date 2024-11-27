using System;
using System.Linq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

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
                    Console.Error.WriteLine("El usuario receptor no está conectado.");
                    return false;
                }
                if (jugador.InvitacionPartidaCallBack is IInvitacionPartidaCallback callback)
                {
                    InvitacionPartida invitacion = new InvitacionPartida(gamertagEmisor, codigoSala, gamertagReceptor);
                    callback?.RecibirInvitacionCallback(invitacion);
                    Console.WriteLine($"Invitación enviada a {gamertagReceptor} para unirse a la sala {codigoSala}");
                    return true;
                }
                Console.Error.WriteLine("El usuario receptor no está conectado.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al enviar invitación: {ex.Message}");
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
                    Console.WriteLine($"Canal de invitaciones abierto para el usuario {usuarioRemitente.Nombre}");
                }
            }
            catch (Exception ex)
            {
                if (jugadoresConectadosDiccionario.TryGetValue(usuarioRemitente.IdUsuario, out UsuarioContexto remitente))
                {
                    lock (remitente)
                    {
                        remitente.InvitacionPartidaCallBack = null;
                    }
                }
                Console.Error.WriteLine($"Error al abrir canal de invitaciones para {usuarioRemitente.Nombre}: {ex.Message}");
            }
        }
    }
}
