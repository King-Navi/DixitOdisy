using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    public class MediadorPartida : IMediadorImagen
    {
        private readonly ReaderWriterLockSlim bloqueo = new ReaderWriterLockSlim();
        private ConcurrentBag<string> ImagenesUsadas { get; set; } = new ConcurrentBag<string>();
        private static readonly ThreadLocal<Random> aleatorio = new ThreadLocal<Random>(() => new Random());
        private readonly Lazy<string[]> archivosCache;

        public int CartasRestantes { get; private set; } = 0;

        public MediadorPartida(TematicaPartida tematica)
        {
            CartasRestantes = Directory.GetFiles(Rutas.CalcularRutaImagenes(tematica), Rutas.EXTENSION_TODO_ARCHIVO_JPG).Length;
            archivosCache = new Lazy<string[]>(() => Directory.GetFiles(Rutas.CalcularRutaImagenes(tematica)));

        }

        public (string RutaCompleta, string NombreArchivo) ObtenerRutaCompeltaYNombreImagen()
        {
            bloqueo.EnterWriteLock();
            try
            {
                var archivosRestantes = ObtenerArchivosCache()
                    .Where(archivo => !ImagenesUsadas
                    .Contains(Path.GetFileNameWithoutExtension(archivo)))
                    .ToArray();
                
                if (archivosRestantes.Length == 0 || archivosRestantes == null)
                {
                    throw new InvalidOperationException("No hay más imágenes disponibles.");
                }
                string rutaCompletaAleatoria = archivosRestantes[aleatorio.Value.Next(archivosRestantes.Length)];
                string nombreSinExtension = Path.GetFileNameWithoutExtension(rutaCompletaAleatoria);
                ImagenesUsadas.Add(nombreSinExtension);
                CartasRestantes--;
                return (rutaCompletaAleatoria, nombreSinExtension);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
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
                bloqueo.ExitWriteLock();
            }
            return (null, null);
        }

        public List<(string RutaCompleta, string NombreArchivo)> ObtenerMultiplesRutasYNombres(int cantidad)
        {
            bloqueo.EnterWriteLock();
            try
            {
                var archivosRestantes = ObtenerArchivosCache()
                    .Where(archivo => !ImagenesUsadas
                    .Contains(Path.GetFileNameWithoutExtension(archivo)))
                    .ToArray();

                if (archivosRestantes.Length == 0 || archivosRestantes == null)
                {
                    throw new InvalidOperationException("No hay más imágenes disponibles.");
                }

                if (cantidad > archivosRestantes.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad solicitada excede las imágenes disponibles.");
                }

                var seleccionadas = new List<(string RutaCompleta, string NombreArchivo)>();
                for (int iteracion = 0; iteracion < cantidad; iteracion++)
                {
                    string rutaCompletaAleatoria = archivosRestantes[aleatorio.Value.Next(archivosRestantes.Length)];
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(rutaCompletaAleatoria);
                    seleccionadas.Add((rutaCompletaAleatoria, nombreSinExtension));
                    ImagenesUsadas.Add(nombreSinExtension);
                    archivosRestantes = archivosRestantes
                        .Where(archivo => Path.GetFileNameWithoutExtension(archivo) != nombreSinExtension)
                        .ToArray();
                }

                CartasRestantes -= seleccionadas.Count;
                return seleccionadas;
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (ArgumentOutOfRangeException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            finally
            {
                bloqueo.ExitWriteLock();
            }

            return null;
        }

        public int ObtenerCartasRestantes()
        {
            bloqueo.EnterReadLock();
            try
            {
                return CartasRestantes;
            }
            finally
            {
                bloqueo.ExitReadLock();
            }
        }

        public List<String> ObtenerRutasPorNombreArchivo( List<string> listaNombreArhivos)
        {
            List<string> rutasCompletas = new List<string>();
            try
            {
                string[] archivosCache = ObtenerArchivosCache();
                if (archivosCache == null)
                {
                    return new List<string>();
                }
                var archivoRutaMapa = archivosCache.ToDictionary(ruta => Path.GetFileNameWithoutExtension(ruta), ruta => ruta);

                foreach (var nombreArchivo in listaNombreArhivos)
                {
                    if (archivoRutaMapa.TryGetValue(nombreArchivo, out var rutaCompleta))
                    {
                        rutasCompletas.Add(rutaCompleta);
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return rutasCompletas;
        }

        private string[] ObtenerArchivosCache()
        {
            string[] archivos = new string[0];
            try
            {
                archivos = archivosCache.Value;
                return archivos;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return archivos;
        }
    }
}
