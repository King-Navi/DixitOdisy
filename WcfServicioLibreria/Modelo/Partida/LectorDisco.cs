using System;
using System.Collections.Concurrent;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;

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

        public void EncolarLecturaEnvio(string archivoPath, IImagenCallback callback, bool usarGrupo = false)
        {
            LecturaTrabajo nuevotrabajo = new LecturaTrabajo(archivoPath, callback, usarGrupo);
            if (callback != null)
            {
                colaLectura.Add(nuevotrabajo);
            }
        }

        private async Task ProcesarColaLecturaEnvio(CancellationToken cancelacion)
        {
            try
            {
                while (!detener || !colaLectura.IsCompleted)
                {
                    foreach (var lecturaTrabajo in colaLectura.GetConsumingEnumerable())
                    {
                        try
                        {
                            byte[] imagenBytes;
                            using (var fileStream = new FileStream(lecturaTrabajo.ArchivoPath, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, useAsync: true))
                            {
                                imagenBytes = new byte[fileStream.Length];
                                await fileStream.ReadAsync(imagenBytes, 0, (int)fileStream.Length);
                            }
                            string nombreSinExtension = Path.GetFileNameWithoutExtension(lecturaTrabajo.ArchivoPath);
                            using (var imagenStream = new MemoryStream(imagenBytes))
                            {
                                var imagenCarta = new ImagenCarta
                                {
                                    IdImagen = nombreSinExtension,
                                    ImagenStream = imagenStream
                                };
                                try
                                {
                                    EvaluarCanalValido(lecturaTrabajo.Callback);
                                    if (lecturaTrabajo.UsarGrupo)
                                    {
                                        lecturaTrabajo.Callback.RecibirGrupoImagenCallback(imagenCarta);
                                        Console.WriteLine($"El {idLector} envio la imagen {nombreSinExtension} en modo grupo");
                                    }
                                    else
                                    {
                                        lecturaTrabajo.Callback.RecibirImagenCallback(imagenCarta);
                                        Console.WriteLine($"El {idLector} envio la imagen {nombreSinExtension} ");
                                    }
                                }
                                catch (CommunicationException excecpione)
                                {
                                    ManejadorExcepciones.ManejarErrorException(excecpione);
                                }
                                catch (Exception excecpione)
                                {
                                    ManejadorExcepciones.ManejarErrorException(excecpione);
                                }
                            }
                        }
                        catch (Exception excecpione)
                        {
                            ManejadorExcepciones.ManejarErrorException(excecpione);
                            Console.WriteLine($"Error al leer la imagen: {excecpione.Message}");
                        }
                    }
                }
            }
            catch (Exception excecpiones)
            {
                ManejadorExcepciones.ManejarFatalException( excecpiones);
                Console.WriteLine($"[{idLector}] Procesamiento cancelado.");
            }
            finally
            {
                Console.WriteLine($"[{idLector}] Tarea finalizada.");
            }
        }

        private void EvaluarCanalValido(IImagenCallback callback)
        {
            if (callback is ICommunicationObject canal)
            {
                if (canal.State == CommunicationState.Opened)
                {
                    return;
                }
                else
                {
                    throw new CommunicationException();
                }
            }
            else
            {
                throw new CommunicationException();
            };
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
            catch (AggregateException excepcion)
            {
                foreach (var operacion in excepcion.InnerExceptions)
                {
                    if (operacion is OperationCanceledException)
                        Console.WriteLine("Tarea cancelada correctamente.");
                    else
                        Console.WriteLine($"Error en DetenerLectura: {operacion.Message}");
                }
            }
            finally
            {
                cancelarToken.Dispose(); 
            }
        }
    }
}
