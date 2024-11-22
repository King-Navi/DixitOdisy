using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    internal class LectorDiscoOrquestador
    {

        private readonly List<LectorDisco> lectoresDisco = new List<LectorDisco>();
        private readonly int maxLectores;
        private int indiceActual = 0;
        private readonly SemaphoreSlim semaforoAsignacion = new SemaphoreSlim(1, 1);
        public bool Desechado { get; private set; } = false;

        public LectorDiscoOrquestador(int cantidadLectores)
        {
            if (cantidadLectores > 6)
            {
                cantidadLectores = 6;
            }
            maxLectores = cantidadLectores;
            for (int i = 0; i < maxLectores; i++)
            {
                lectoresDisco.Add(new LectorDisco(i));
            }
        }

        public void AsignarTrabajo(string archivoPath, IPartidaCallback callback, bool usarGrupo = false)
        {
            var lectorMenosOcupado = lectoresDisco.OrderBy(l => l.ColaCount).First();
            lectorMenosOcupado.EncolarLecturaEnvio(archivoPath, callback, usarGrupo);
        }

        public async Task AsignarTrabajoRoundRobinAsync(string archivoPath, IPartidaCallback callback)
        {
            await semaforoAsignacion.WaitAsync(); 
            try
            {
                var lectorSeleccionado = lectoresDisco[indiceActual];
                indiceActual = (indiceActual + 1) % lectoresDisco.Count;
                lectorSeleccionado.EncolarLecturaEnvio(archivoPath, callback);
            }
            finally
            {
                semaforoAsignacion.Release();
            }
        }

        public void DetenerLectores()
        {
            foreach (var lector in lectoresDisco)
            {
                lector.DetenerLectura();
            }
        }

        public void LiberarRecursos()
        {
            if (!Desechado)
            {
                DetenerLectores();
                lectoresDisco.Clear();
                Desechado = true;
            }

        }
    }
}
