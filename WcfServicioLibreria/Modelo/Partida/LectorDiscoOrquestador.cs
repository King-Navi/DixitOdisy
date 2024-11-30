using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    internal class LectorDiscoOrquestador
    {

        private readonly List<LectorDisco> lectoresDisco = new List<LectorDisco>();
        private readonly int MAXIMO_LECTORESDISCO;
        private readonly int LECTORESDISCO_CERO;
        private int indiceActual = 0;
        private readonly SemaphoreSlim semaforoAsignacion = new SemaphoreSlim(1, 1);
        public bool Desechado { get; private set; } = false;

        public LectorDiscoOrquestador(int cantidadLectores)
        {
            if (cantidadLectores > MAXIMO_LECTORESDISCO || cantidadLectores <= LECTORESDISCO_CERO)
            {
                for (int i = 0; i < cantidadLectores; i++)
                {
                    lectoresDisco.Add(new LectorDisco(i));

                }
            }
            else
            {
                for (int i = 0; i < cantidadLectores; i++)
                {
                    lectoresDisco.Add(new LectorDisco(i));
                }
            }           
        }

        public void AsignarTrabajo(string archivoPath, IPartidaCallback callback, bool usarGrupo = false)
        {
            var lectorMenosOcupado = lectoresDisco.OrderBy(busqueda => busqueda.ColaCount).First();
            lectorMenosOcupado.EncolarLecturaEnvio(archivoPath, callback, usarGrupo);
        }

        public async Task AsignarTrabajoRoundRobinAsync(string archivoPath, IPartidaCallback callback)
        {
            await semaforoAsignacion.WaitAsync();
            try
            {
                if (lectoresDisco.Count == 0)
                {
                    ManejadorExcepciones.ManejarFatalException(new InvalidOperationException("No hay lectores disponibles para asignar el trabajo."));
                    throw new InvalidOperationException();
                }

                indiceActual = indiceActual % lectoresDisco.Count;
                var lectorSeleccionado = lectoresDisco[indiceActual];
                lectorSeleccionado.EncolarLecturaEnvio(archivoPath, callback);
            }
            finally
            {
                semaforoAsignacion.Release();
            }
        }

        public void LiberarRecursos()
        {
            semaforoAsignacion.Wait();
            try
            {
                if (!Desechado)
                {
                    foreach (var lector in lectoresDisco)
                    {
                        lector.DetenerLectura();
                    }
                    lectoresDisco.Clear();
                    Desechado = true;
                }
            }
            finally
            {
                semaforoAsignacion.Release();
            }
        }
    }
}
