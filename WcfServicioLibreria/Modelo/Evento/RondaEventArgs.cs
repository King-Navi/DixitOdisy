using System.Collections.Generic;

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
