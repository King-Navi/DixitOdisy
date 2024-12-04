using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Utilidades
{
    public interface ILectorDiscoOrquestador
    {
        void AsignarTrabajo(string archivoRutaCompleta, IImagenCallback callback, bool usarGrupo = false);
        Task AsignarTrabajoRoundRobinAsync(string archivoRutaCompleta, IImagenCallback callback, bool usarGrupo = false);
        void LiberarRecursos();
    }
}
