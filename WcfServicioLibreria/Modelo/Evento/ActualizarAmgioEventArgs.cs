using System;
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
