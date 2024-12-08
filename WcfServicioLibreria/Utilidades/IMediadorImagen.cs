using System;
using System.Collections.Generic;

namespace WcfServicioLibreria.Utilidades
{
    public interface IMediadorImagen
    {
        (string RutaCompleta, string NombreArchivo) ObtenerRutaCompeltaYNombreImagen();
        List<(string RutaCompleta, string NombreArchivo)> ObtenerMultiplesRutasYNombres(int cantidad);
        int ObtenerCartasRestantes();
        List<String> ObtenerRutasPorNombreArchivo(List<string> listaNombreArhivos);
    }
}
