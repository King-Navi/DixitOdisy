using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using static System.Net.Mime.MediaTypeNames;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioInvitacionPartida
    {
        public bool EnviarInvitacion(string gamertagEmisor, string codigoSala, string gamertagReceptor)
        {
            try
            {
                // Obtener el canal de callback para el receptor de la invitación.
                IInvitacionPartidaCallback callback = OperationContext.Current.GetCallbackChannel<IInvitacionPartidaCallback>();

                //crear una invitacion
                //callback.RecibirInvitacion(invitacion);

                return true; // Indicar que el envío fue exitoso.
            }
            catch (Exception ex)
            {
                // Manejo de errores.
                Console.Error.WriteLine($"Error al enviar invitación: {ex.Message}");
                return false;
            }
        }

    }
}
