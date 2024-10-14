using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
