using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class InvitacionesPartida
    {
        private readonly List<InvitacionPartida> _listaInvitaciones;

        public InvitacionesPartida()
        {
            _listaInvitaciones = new List<InvitacionPartida>
        {
            new InvitacionPartida { CodigoSala = "12345", GamertagEmisor = "Solicitud 1", GamertagReceptor = "Descripción de la solicitud 1" },
            new InvitacionPartida { CodigoSala = "532135", GamertagEmisor = "Solicitud 2", GamertagReceptor = "Descripción de la solicitud 2" }
        };
        }

        public IEnumerable<InvitacionPartida> ObtenerSolicitudes()
        {
            return _listaInvitaciones;
        }

        public void AceptarSolicitud(InvitacionPartida solicitud)
        {
            if (solicitud != null)
            {
                Console.WriteLine($"Solicitud aceptada: IdSala = {solicitud.CodigoSala}");
                _listaInvitaciones.Remove(solicitud);
            }
        }

        public void EliminarSolicitud(InvitacionPartida solicitud)
        {
            if (solicitud != null)
            {
                Console.WriteLine($"Solicitud eliminada: IdSala = {solicitud.CodigoSala}");
                _listaInvitaciones.Remove(solicitud);
            }
        }

    }

}
