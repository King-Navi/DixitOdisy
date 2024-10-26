using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Evento
{
    internal class PartidaVaciaEventArgs : EventArgs
    {
        public DateTime HoraFecha { get; set; }
        public Partida Partida { get; set; }

        public PartidaVaciaEventArgs(DateTime horaFecha, Partida idSala)
        {
            HoraFecha = horaFecha;
            Partida = idSala;
        }
    }
}
