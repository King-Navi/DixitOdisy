using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Evento
{
    public class MultiChatVacioEventArgs : EventArgs
    {
        public DateTime HoraFecha { get; set; }
        public MultiChat Chat { get; set; }

        public MultiChatVacioEventArgs(DateTime horaFecha, MultiChat chat)
        {
            HoraFecha = horaFecha;
            Chat = chat;
        }
    }
}
