using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo.Evento
{
    public class RondaEventArgs
    {
        public List<string> RutaImagenes { get; set; }
        public RondaEventArgs(List<string> _rutaImagenes)
        {
            RutaImagenes = _rutaImagenes;
        }
    }
    

}
