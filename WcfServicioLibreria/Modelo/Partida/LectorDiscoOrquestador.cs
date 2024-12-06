using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    internal class LectorDiscoOrquestador : ILectorDiscoOrquestador
    {

        private readonly List<LectorDisco> lectoresDisco = new List<LectorDisco>();
        private const int MAXIMO_LECTORESDISCO = 6;
        private const int LECTORESDISCO_CERO = 0;
        private const int LECTORESDISCO_POR_DEFECTO = 2;
        private int indiceActual = 0;
        private readonly SemaphoreSlim semaforoAsignacion = new SemaphoreSlim(1, 1);
        public const string MENSAJE_OPERACION_INVALIDA = "No hay lectores disponibles para asignar el trabajo.";
        public bool Desechado { get; private set; } = false;

        public LectorDiscoOrquestador(int cantidadLectores)
        {
            if (cantidadLectores > MAXIMO_LECTORESDISCO || cantidadLectores <= LECTORESDISCO_CERO)
            {
                for (int iteracion = 0; iteracion < LECTORESDISCO_POR_DEFECTO; iteracion++)
                {
                    lectoresDisco.Add(new LectorDisco(iteracion));
                }
            }
            else
            {
                for (int iteracion = 0; iteracion < cantidadLectores; iteracion++)
                {
                    lectoresDisco.Add(new LectorDisco(iteracion));
                }
            }           
        }

        public void AsignarTrabajo(string archivoPath, IImagenCallback callback, bool usarGrupo = false)
        {
            var lectorMenosOcupado = lectoresDisco.OrderBy(busqueda => busqueda.ColaCount).First();
            lectorMenosOcupado.EncolarLecturaEnvio(archivoPath, callback, usarGrupo);
        }

        public async Task AsignarTrabajoRoundRobinAsync(string archivoPath, IImagenCallback callback, bool usarGrupo = false)
        {
            await semaforoAsignacion.WaitAsync();
            try
            {
                if (lectoresDisco.Count == 0)
                {
                    ManejadorExcepciones.ManejarExcepcionFatal(new InvalidOperationException(MENSAJE_OPERACION_INVALIDA));
                    throw new InvalidOperationException();
                }
                var lectorSeleccionado = lectoresDisco[indiceActual];
                indiceActual = (indiceActual + 1) % lectoresDisco.Count;
                lectorSeleccionado.EncolarLecturaEnvio(archivoPath, callback, usarGrupo);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
