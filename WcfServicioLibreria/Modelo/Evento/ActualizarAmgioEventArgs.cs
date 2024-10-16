using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Evento
{
    public class ActualizarAmgioEventArgs : EventArgs
    {
        public DateTime HoraFecha { get; set; }
        public Amigo Amigo { get; set; }
        public ActualizarAmgioEventArgs(Amigo amigo, DateTime horaFecha)
        {
            HoraFecha = horaFecha;
            Amigo = amigo;
        }

    }
}
