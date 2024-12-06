using ChatGPTLibreria.ModelosJSON;
using ChatGPTLibreria;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using System.Collections.Concurrent;
using System.Threading;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Utilidades;
using System.ServiceModel;
using WcfServicioLibreria.Modelo.Evento;

namespace WcfServicioLibreria.Modelo
{
    public class ManejadorImagen : IManejadorImagen
    {

        private const int LIMITE_CARTAS_MINIMO = 0;
        private const int LECTORESDISCO = 6;
        private const string PRIMER_PARTE_ENTRADA = "Genera una imagen basada en la tematica";
        private const string SEGUNDA_PARTE_ENTRADA = "Debe ser rectangular y vertical, de alta calidad tiene que ser muy buena por que es para el profe juan carlos";
        private const string EXTENSION_PUNTO_JPG = ".jpg";
        private const int TIEMPO_AUMENTO_ENTRADA_SEGUNDOS = 4;
        private static int tiempoEspera = 0;
        private static readonly int tiempoMinimo = 16;
        private static readonly object lockTiempoEspera = new object();
        private const int TIEMPO_ESPERA_MILISEGUNDOS = 1000;
        private IMediadorImagen mediadorImagen;
        private readonly TematicaPartida tematica;
        private IEscribirDisco escritor;
        private readonly SemaphoreSlim semaphoreLectura = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEnvioGrupo = new SemaphoreSlim(1, 1);
        private static readonly HttpClient httpCliente = new HttpClient();
        private readonly string rutaImagenes;
        private List<string> listaActualElegida;
        /// <summary>
        /// Precausion: Aumentar esto ocupara significativamente mas recursos
        /// </summary>
        private readonly ILectorDiscoOrquestador lectorDiscoOrquetadorMazo = new LectorDiscoOrquestador(LECTORESDISCO);
        private readonly ILectorDiscoOrquestador lectorDiscoOrquetadorGrupo = new LectorDiscoOrquestador(LECTORESDISCO);
        public ManejadorImagen(IEscribirDisco _escritor, IMediadorImagen _mediadorImagen, TematicaPartida _tematica)
        {
            escritor = _escritor;
            mediadorImagen = _mediadorImagen;
            tematica = _tematica;
            rutaImagenes = Rutas.CalcularRutaImagenes(tematica);
        }

        #region Imagenes
        public async Task<bool> EnviarImagenAsync(IImagenCallback callback)
        {
            return await CalcularNuevaImagenAsync(callback);
        }

        private async Task<bool> CalcularNuevaImagenAsync(IImagenCallback callback)
        {
            bool resultado = false;
            int cartasRestantes = mediadorImagen.ObtenerCartasRestantes();
            try
            {
                if (cartasRestantes <= LIMITE_CARTAS_MINIMO)
                {
                    resultado = await SolicitarImagenChatGPTAsync(callback);
                }
                else
                {
                    resultado = await LeerImagenDiscoAsync( callback);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return true;
        }

        private async Task<bool> LeerImagenDiscoAsync( IImagenCallback callback)
        {
            await semaphoreLectura.WaitAsync();

            try
            {
                var (rutaCompleta, nombreArchivo) = mediadorImagen.ObtenerRutaCompeltaYNombreImagen();
                if (String.IsNullOrEmpty(rutaCompleta) || String.IsNullOrEmpty(nombreArchivo))
                {
                    return await SolicitarImagenChatGPTAsync(callback);
                }
                await lectorDiscoOrquetadorMazo.AsignarTrabajoRoundRobinAsync(rutaCompleta, callback);
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            finally
            {
                semaphoreLectura.Release();
            }
            return false;
        }


        private async Task<bool> SolicitarImagenChatGPTAsync( IImagenCallback callback)
        {
            try
            {
                var respuesta = await GenerarImagenDesdeChatGPTAsync();
                if (respuesta == null)
                {
                    return false;
                }

                var imagenCarta = ProcesarImagenRespuesta(respuesta);
                if (imagenCarta == null)
                {
                    return false;
                }

                EnviarImagenCallback(callback, imagenCarta);
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                return false;
            }
        }

        private async Task<ImagenRespuesta64JSON> GenerarImagenDesdeChatGPTAsync()
        {
            var imagenPedido = new ImagenPedido64JSON($"{PRIMER_PARTE_ENTRADA} {tematica}. {SEGUNDA_PARTE_ENTRADA}");
            var solicitarImagen = new SolicitarImagen();
            var respuesta = await solicitarImagen.EjecutarImagenPrompt64JSON(imagenPedido, httpCliente);

            return respuesta?.ImagenDatosList != null && respuesta.ImagenDatosList.Any()
                ? respuesta
                : null;
        }

        private ImagenCarta ProcesarImagenRespuesta(ImagenRespuesta64JSON respuesta)
        {
            try
            {
                var imagenBytes = Convert.FromBase64String(respuesta.ImagenDatosList[0].Base64Imagen);
                var rutaDestino = Path.Combine(rutaImagenes, $"{Guid.NewGuid()}{EXTENSION_PUNTO_JPG}");

                using (var memoryStream = new MemoryStream(imagenBytes))
                {
                    escritor.EncolarEscritura(memoryStream, rutaDestino);

                    return new ImagenCarta
                    {
                        ImagenStream = memoryStream,
                        IdImagen = Path.GetFileNameWithoutExtension(rutaDestino)
                    };
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                return null;
            }
        }

        private void EnviarImagenCallback(IImagenCallback callback, ImagenCarta imagenCarta)
        {
            if (callback == null)
            {
                return;
            }
            try
            {
                callback?.RecibirImagenCallback(imagenCarta);
            }
            catch (FormatException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
        }

        public void EnMostrarImagenes(object emisor, RondaEventArgs evento)
        {
            listaActualElegida = evento.RutaImagenes;
        }

        public async Task MostrarGrupoCartasAsync(IImagenCallback callback)
        {
            int esperaActual;
            lock (lockTiempoEspera)
            {
                esperaActual = tiempoEspera;
                if (tiempoEspera > tiempoMinimo)
                {
                    tiempoEspera += TIEMPO_AUMENTO_ENTRADA_SEGUNDOS;
                }
            }
            await Task.Delay(esperaActual * TIEMPO_ESPERA_MILISEGUNDOS);
            await semaphoreEnvioGrupo.WaitAsync();
            List<string> rutasCompletas = new List<string>();
            try
            {
                string[] archivosCache = mediadorImagen.ObtenerArchivosCache();
                if (archivosCache == null)
                {
                    return;
                }
                var archivoRutaMapa = archivosCache.ToDictionary(ruta => Path.GetFileNameWithoutExtension(ruta), ruta => ruta);

                foreach (var nombreArchivo in listaActualElegida)
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

            try
            {
                foreach (var rutaImagen in rutasCompletas)
                {
                    try
                    {
                        await lectorDiscoOrquetadorGrupo?.AsignarTrabajoRoundRobinAsync(rutaImagen, callback, true);
                    }
                    catch (TimeoutException excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
                    }
                    catch (CommunicationException excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionError(excepcion);
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            semaphoreEnvioGrupo.Release();
        }




        public void LiberarRecursos()
        {
            lectorDiscoOrquetadorMazo.LiberarRecursos();
            lectorDiscoOrquetadorGrupo.LiberarRecursos();
            mediadorImagen = null;
        }

        internal void SeTerminoLectura(object sender, EventArgs e)
        {
            LiberarRecursos();
        }



        #endregion Imagenes

    }
}
