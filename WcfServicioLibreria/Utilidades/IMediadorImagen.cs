using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public interface IMediadorImagen
    {
        (string RutaCompleta, string NombreArchivo) ObtenerRutaCompeltaYNombreImagen();
        int ObtenerCartasRestantes();
        string[] ObtenerArchivosCache();
    }
}
