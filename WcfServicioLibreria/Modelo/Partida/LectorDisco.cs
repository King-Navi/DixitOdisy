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
        private readonly BlockingCollection<LecturaTrabajo> colaLectura = new BlockingCollection<LecturaTrabajo>();
        private readonly Task tareaLectura;
        private bool detener = false;

        public LectorDisco()
        {
            tareaLectura = Task.Run( async () => await ProcesarColaLecturaEnvio());
        }

        public void EncolarLecturaEnvio(string archivoPath, IPartidaCallback callback, bool usarGrupo = false)
        {
            LecturaTrabajo nuevotrabajo = new LecturaTrabajo(archivoPath, callback, usarGrupo);
            if (callback != null)
            colaLectura.Add(nuevotrabajo);
        }

        private async Task ProcesarColaLecturaEnvio()
        {
            while (!detener || !colaLectura.IsCompleted)
            {
                foreach (var lecturaTrabajo in colaLectura.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Procesando archivo: {lecturaTrabajo.ArchivoPath}");

                    try
                    {
                        // Leer la imagen del disco de forma asíncrona
                        byte[] imagenBytes;
                        using (var fileStream = new FileStream(lecturaTrabajo.ArchivoPath, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, useAsync: true))
                        {
                            imagenBytes = new byte[fileStream.Length];
                            await fileStream.ReadAsync(imagenBytes, 0, (int)fileStream.Length);
                        }

                        // Obtener el nombre del archivo sin la extensión
                        string nombreSinExtension = Path.GetFileNameWithoutExtension(lecturaTrabajo.ArchivoPath);

                        // Crear el objeto de la imagen
                        using (var imagenStream = new MemoryStream(imagenBytes))
                        {
                            var imagenCarta = new ImagenCarta
                            {
                                IdImagen = nombreSinExtension,
                                ImagenStream = imagenStream
                            };

                            // Intentar enviar la imagen al callback
                            try
                            {
                                if (lecturaTrabajo.UsarGrupo)
                                {
                                    lecturaTrabajo.Callback.RecibirGrupoImagenCallback(imagenCarta);
                                }
                                else
                                {
                                    lecturaTrabajo.Callback.RecibirImagenCallback(imagenCarta);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error en callback: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al leer la imagen: {ex.Message}");
                    }
                    finally
                    {
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
