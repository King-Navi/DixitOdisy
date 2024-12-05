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
        long TAMANO_MAXIMO_BYTES = 5 * 1024 * 1024;
        private readonly BlockingCollection<LecturaTrabajo> colaLectura = new BlockingCollection<LecturaTrabajo>();
        private readonly CancellationTokenSource cancelarToken = new CancellationTokenSource(); 
        private readonly Task tareaLectura;
        private bool detener = false;
        private bool lecturaYaDetenida = false; 
        private readonly int CERO = 0; 
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
                    foreach (var lecturaTrabajo in colaLectura.GetConsumingEnumerable(cancelacion))
                    {
                        try
                        {  
                            byte[] imagenBytes;
                            using (var fileStream = new FileStream(lecturaTrabajo.ArchivoPath, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, useAsync: true))
                            {
                                if (fileStream.Length > TAMANO_MAXIMO_BYTES)
                                {
                                    throw new ArgumentException($"El archivo {lecturaTrabajo.ArchivoPath} excede el tamaño permitido de {TAMANO_MAXIMO_BYTES / (1024 * 1024)} MB.");
                                }
                                imagenBytes = new byte[fileStream.Length];
                                int bytesTotalesLeidos = 0;

                                while (bytesTotalesLeidos < fileStream.Length)
                                {
                                    int bytesLeidos = await fileStream.ReadAsync(imagenBytes, bytesTotalesLeidos, (int)(fileStream.Length - bytesTotalesLeidos), cancelacion);
                                    if (bytesLeidos == 0)
                                    {
                                        break;
                                    }

                                    bytesTotalesLeidos += bytesLeidos;
                                }
                                if (bytesTotalesLeidos != fileStream.Length)
                                {
                                    throw new IOException($"No se pudieron leer todos los bytes del archivo {lecturaTrabajo.ArchivoPath}.");
                                }
                            }
                            string nombreSinExtension = Path.GetFileNameWithoutExtension(lecturaTrabajo.ArchivoPath);
                            using (var imagenStream = new MemoryStream(imagenBytes))
                            {
                                var imagenCarta = new ImagenCarta
                                {
                                    IdImagen = nombreSinExtension,
                                    ImagenStream = imagenStream
                                };
                                EnviarImagenHilo(lecturaTrabajo, imagenCarta);
                            }
                        }
                        catch (Exception excecpione)
                        {
                            ManejadorExcepciones.ManejarExcepcionError(excecpione);
                        }
                    }
                }
            }
            catch (Exception excecpiones)
            {
                ManejadorExcepciones.ManejarExcepcionFatal( excecpiones);
            }
        }

        private void EnviarImagenHilo(LecturaTrabajo lecturaTrabajo , ImagenCarta imagenCarta)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        EvaluarCanalValido(lecturaTrabajo.Callback);
                        if (lecturaTrabajo.UsarGrupo)
                        {
                            lecturaTrabajo.Callback.RecibirGrupoImagenCallback(imagenCarta);
                        }
                        else
                        {
                            lecturaTrabajo.Callback.RecibirImagenCallback(imagenCarta);
                        }
                    }
                    catch (CommunicationException excecpione)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excecpione);
                    }
                    catch (Exception excecpione)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excecpione);
                    }
                });
                
            }
            catch (ArgumentNullException excecpione)
            {
                ManejadorExcepciones.ManejarExcepcionError(excecpione);
            }
            catch (Exception excecpione)
            {
                ManejadorExcepciones.ManejarExcepcionError(excecpione);
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
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excecpione)
            {
                ManejadorExcepciones.ManejarExcepcionError(excecpione);
            }
            finally
            {
                cancelarToken.Dispose(); 
            }
        }
    }
}
