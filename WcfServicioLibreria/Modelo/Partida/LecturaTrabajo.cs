using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    public class LecturaTrabajo
    {

        public List<string> ArchivoRutas { get; }
        public IImagenesTableroCallback CallbackImagenesTablero { get; } = null;
        public IImagenMazoCallback CallbackImagenesMazo { get; } = null;

        public LecturaTrabajo(List<string> rutas, IImagenesTableroCallback callbackImagenesTablero)
        {
            ArchivoRutas = rutas;
            CallbackImagenesTablero = callbackImagenesTablero;
        }

        public LecturaTrabajo(List<string> rutas, IImagenMazoCallback callbackImagenesMazo)
        {
            ArchivoRutas = rutas;
            CallbackImagenesMazo = callbackImagenesMazo;
        }
    }
}
