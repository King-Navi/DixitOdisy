using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    internal class LectorDisco
    {
        private readonly BlockingCollection<(string archivoPath, IPartidaCallback callback)> colaLectura = new BlockingCollection<(string, IPartidaCallback)>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Task tareaLectura;
        private bool detener = false;

        public LectorDisco()
        {
            tareaLectura = Task.Run(() => ProcesarColaLecturaEnvio());
        }

        public void EncolarLecturaEnvio(string archivoPath, IPartidaCallback callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            colaLectura.Add((archivoPath, callback));
        }

        private async Task ProcesarColaLecturaEnvio()
        {
            while (!detener || colaLectura.Count > 0)
            {
                foreach (var (archivoPath, callback) in colaLectura.GetConsumingEnumerable())
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        // Leer la imagen del disco
                        byte[] imagenBytes = await Task.Run(() => File.ReadAllBytes(archivoPath));
                        string nombreSinExtension = Path.GetFileNameWithoutExtension(archivoPath);

                        // Crear objeto de imagen
                        var imagenCarta = new ImagenCarta
                        {
                            IdImagen = nombreSinExtension,
                            ImagenStream = new MemoryStream(imagenBytes)
                        };

                        // Enviar la imagen al callback
                        try
                        {
                            callback.RecibirImagenCallback(imagenCarta);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error en callback: {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al leer la imagen: {ex.Message}");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            }
        }

        public void DetenerLectura()
        {
            detener = true;
            colaLectura.CompleteAdding();
            tareaLectura.Wait(); // O espera asíncronamente con `await tareaLectura` si haces el método asíncrono.
        }
    }
}
