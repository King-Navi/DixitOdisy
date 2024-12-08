using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    public class LectorDisco : ILectorDisco
    {
        public const long TAMANO_MAXIMO_BYTES = 5 * 1024 * 1024;
        private readonly int CERO = 0;
        private readonly int IMAGEN_UNICA = 1;

        public async Task<bool> ProcesarColaLecturaEnvio(LecturaTrabajo lecturaTrabajo)
        {
            try
            {
                List<ImagenCarta> listaImagenes = new List<ImagenCarta>();
                foreach (var ruta in lecturaTrabajo.ArchivoRutas)
                {
                    var imagenBytes = await LeerArchivoAsync(ruta);
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(ruta);

                    var imagenCarta = new ImagenCarta
                    {
                        IdImagen = nombreSinExtension,
                        ImagenStream = imagenBytes
                    };
                    listaImagenes.Add(imagenCarta);
                }
                EnviarImagenHilo(lecturaTrabajo, listaImagenes);
                return true;
            }
            catch (Exception excecpione)
            {
                ManejadorExcepciones.ManejarExcepcionError(excecpione);
            }
            return false;
        }

        public async Task<byte[]> LeerArchivoAsync(string rutaArchivo)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    throw new FileNotFoundException($"El archivo {rutaArchivo} no existe.");
                }
                using (var fileStream = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, useAsync: true))
                {
                    if (fileStream.Length > TAMANO_MAXIMO_BYTES)
                    {
                        throw new ArgumentException($"El archivo {rutaArchivo} excede el tamaño permitido de 5 MB.");
                    }

                    byte[] imagenBytes = new byte[fileStream.Length];
                    int bytesTotalesLeidos = CERO;

                    while (bytesTotalesLeidos < fileStream.Length)
                    {
                        int bytesLeidos = await fileStream.ReadAsync(imagenBytes, bytesTotalesLeidos, (int)(fileStream.Length - bytesTotalesLeidos));
                        if (bytesLeidos == CERO)
                        {
                            break;
                        }

                        bytesTotalesLeidos += bytesLeidos;
                    }

                    if (bytesTotalesLeidos != fileStream.Length)
                    {
                        throw new IOException($"No se pudieron leer todos los bytes del archivo {rutaArchivo}.");
                    }

                    return imagenBytes;
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EnviarImagenHilo(LecturaTrabajo lecturaTrabajo, List<ImagenCarta> imagenCarta)
        {
            try
            {
                if (lecturaTrabajo.CallbackImagenesTablero != null)
                {
                    EnviarImagenesTablero(lecturaTrabajo, imagenCarta);
                }
                else if (lecturaTrabajo.CallbackImagenesMazo != null)
                {
                    if (imagenCarta.Count == IMAGEN_UNICA)
                    {
                        EnviarImagenMazo(lecturaTrabajo, imagenCarta);
                    }
                    else
                    {
                        EnviarImagenesMazo(lecturaTrabajo, imagenCarta);
                    }
                }
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

        private void EnviarImagenesMazo(LecturaTrabajo lecturaTrabajo, List<ImagenCarta> imagenCarta)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        EvaluarCanalValido(lecturaTrabajo.CallbackImagenesMazo);
                        lecturaTrabajo.CallbackImagenesMazo.RecibirVariasImagenCallback(imagenCarta);
                    }
                    catch (ArgumentOutOfRangeException excecpione)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excecpione);
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
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnviarImagenMazo(LecturaTrabajo lecturaTrabajo, List<ImagenCarta> imagenCarta)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        EvaluarCanalValido(lecturaTrabajo.CallbackImagenesMazo);
                        lecturaTrabajo.CallbackImagenesMazo.RecibirImagenCallback(imagenCarta[0]);
                    }
                    catch (ArgumentOutOfRangeException excecpione)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excecpione);
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
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EnviarImagenesTablero(LecturaTrabajo lecturaTrabajo, List<ImagenCarta> imagenCarta)
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        EvaluarCanalValido(lecturaTrabajo.CallbackImagenesTablero);
                        lecturaTrabajo.CallbackImagenesTablero.RecibirGrupoImagenCallback(imagenCarta);
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
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EvaluarCanalValido(IImagenMazoCallback callback)
        {
            if (callback is ICommunicationObject canal)
            {
                if (canal.State == CommunicationState.Opened)
                {
                    return;
                }
            }
            throw new CommunicationException();

        }

        private void EvaluarCanalValido(IImagenesTableroCallback callback)
        {
            if (callback is ICommunicationObject canal)
            {
                if (canal.State == CommunicationState.Opened)
                {
                    return;
                }
            }
            throw new CommunicationException();
        }

    }
}
