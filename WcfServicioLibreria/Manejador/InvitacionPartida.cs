using System;
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
                foreach (var jugador in jugadoresConectadosDiccionario.Values)
                {
                    if (jugador.Nombre.Equals(gamertagReceptor, StringComparison.OrdinalIgnoreCase))
                    {
                        var callback = jugador.InvitacionPartidaCallBack as IInvitacionPartidaCallback;

                        if (callback != null)
                        {
                            InvitacionPartida invitacion = new InvitacionPartida(gamertagEmisor, codigoSala, gamertagReceptor);
                            callback.RecibirInvitacion(invitacion);
                            Console.WriteLine($"Invitación enviada a {gamertagReceptor} para unirse a la sala {codigoSala}");
                            return true;
                        }
                        else
                        {
                            Console.Error.WriteLine("El callback del receptor no es válido.");
                            return false;
                        }
                    }
                }

                Console.Error.WriteLine("El usuario receptor no está conectado.");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al enviar invitación: {ex.Message}");
                return false;
            }
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
                throw;
            }
        }
    }
}
