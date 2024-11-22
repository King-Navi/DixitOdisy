using System;
using WcfServicioLibreria.Modelo;
namespace WcfServicioLibreria.Evento
{
    public class SalaVaciaEventArgs : EventArgs
    {
        public DateTime HoraFecha { get; set; }
        public Sala Sala { get; set; }

        public SalaVaciaEventArgs(DateTime horaFecha, Sala idSala)
        {
            HoraFecha = horaFecha;
            Sala = idSala;
        }
    }
}
