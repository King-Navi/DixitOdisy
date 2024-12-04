namespace WcfServicioLibreria.Utilidades
{
    public interface IMediadorImagen
    {
        (string RutaCompleta, string NombreArchivo) ObtenerRutaCompeltaYNombreImagen();
        int ObtenerCartasRestantes();
        string[] ObtenerArchivosCache();
    }
}
