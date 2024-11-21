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
        private readonly int idLector;
        private readonly BlockingCollection<LecturaTrabajo> colaLectura = new BlockingCollection<LecturaTrabajo>();
        private readonly CancellationTokenSource cancelarToken = new CancellationTokenSource(); 
        private readonly Task tareaLectura;
        private bool detener = false;
        private bool lecturaYaDetenida = false; 
        public int ColaCount => colaLectura.Count;

        public LectorDisco()
        {
            tareaLectura = Task.Run( async () => await ProcesarColaLecturaEnvio(cancelarToken.Token));
        }
        public LectorDisco(int idLector)
        {
            this.idLector = idLector;
            tareaLectura = Task.Run( async () => await ProcesarColaLecturaEnvio(cancelarToken.Token));
        }

        public void EncolarLecturaEnvio(string archivoPath, IPartidaCallback callback, bool usarGrupo = false)
        {
            LecturaTrabajo nuevotrabajo = new LecturaTrabajo(archivoPath, callback, usarGrupo);
            if (callback != null)
            colaLectura.Add(nuevotrabajo);
        }

        private async Task ProcesarColaLecturaEnvio(CancellationToken cancelacion)
        {
            try
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
            catch (Exception)
            {
                Console.WriteLine($"[{idLector}] Procesamiento cancelado.");
            }
            finally
            {
                Console.WriteLine($"[{idLector}] Tarea finalizada.");
            }
        }

        public void DetenerLectura()
        {
            if (lecturaYaDetenida) return; 
            lecturaYaDetenida = true; 

            detener = true;
            cancelarToken.Cancel(); 
            colaLectura.CompleteAdding(); 

            try
            {
                tareaLectura.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    if (e is OperationCanceledException)
                        Console.WriteLine("Tarea cancelada correctamente.");
                    else
                        Console.WriteLine($"Error en DetenerLectura: {e.Message}");
                }
            }
            finally
            {
                cancelarToken.Dispose(); 
            }
        }
    }
}
