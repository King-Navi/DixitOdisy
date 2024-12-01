using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    internal class LecturaTrabajo
    {

        public string ArchivoPath { get; }
        public IImagenCallback Callback { get; }
        public bool UsarGrupo { get; }

        public LecturaTrabajo(string archivoPath, IImagenCallback callback, bool usarGrupo)
        {
            ArchivoPath = archivoPath;
            Callback = callback;
            UsarGrupo = usarGrupo;

        }
    }
}
