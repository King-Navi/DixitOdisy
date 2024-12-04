using System;
using System.Collections.Concurrent;
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
                Console.WriteLine($"Se utilizo el archivo {nombreSinExtension}");
                CartasRestantes--;
                return (rutaCompletaAleatoria, nombreSinExtension);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            finally
            {
                bloqueo.ExitWriteLock();
            }
            return (null, null);
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
        public string[] ObtenerArchivosCache()
        {
            try
            {
                return archivosCache.Value;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return null;
        }
    }
}
