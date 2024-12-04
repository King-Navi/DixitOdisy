﻿using ChatGPTLibreria.ModelosJSON;
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
        private IMediadorImagen mediadorImagen;
        private readonly TematicaPartida tematica;
        private IEscribirDisco escritor;
        private readonly SemaphoreSlim semaphoreLectura = new SemaphoreSlim(1, 1);
        private static readonly HttpClient httpCliente = new HttpClient();
        private readonly string rutaImagenes;
        private List<string> listaActualElegida;
        /// <summary>
        /// Precausion: Aumentar esto ocupara significativamente mas recursos
        /// </summary>
        private readonly ILectorDiscoOrquestador lectorDiscoOrquetador = new LectorDiscoOrquestador(LECTORESDISCO);
        public ManejadorImagen(IEscribirDisco _escritor, IMediadorImagen _mediadorImagen, TematicaPartida _tematica)
        {
            escritor = _escritor;
            mediadorImagen = _mediadorImagen;
            tematica = _tematica;
            rutaImagenes = Rutas.CalcularRutaImagenes(tematica);
        }

        #region Imagenes
        public async Task<bool> EnviarImagen(IImagenCallback callback)
        {
            return await CalcularNuevaImagen(callback);
        }

        private async Task<bool> CalcularNuevaImagen(IImagenCallback callback)
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
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
                    Console.WriteLine($"No quedan imágenes disponibles. Solicitud externa requerida para petion");
                    return await SolicitarImagenChatGPTAsync(callback);
                }
                await lectorDiscoOrquetador.AsignarTrabajoRoundRobinAsync(rutaCompleta, callback);
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
        }

        public void EnMostrarImagenes(object emisor, RondaEventArgs evento)
        {
            listaActualElegida = evento.RutaImagenes;
        }

        public async void MostrarGrupoCartas(IImagenCallback callback)
        {
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
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            foreach (var rutaImagen in rutasCompletas)
            {
                try
                {
                    await lectorDiscoOrquetador?.AsignarTrabajoRoundRobinAsync(rutaImagen, callback, true);
                }
                catch (TimeoutException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (CommunicationException excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
            }
        }




        public void LiberarRecursos()
        {
            lectorDiscoOrquetador.LiberarRecursos();
            mediadorImagen = null;
        }

        internal void SeTerminoLectura(object sender, EventArgs e)
        {
            LiberarRecursos();
        }



        #endregion Imagenes

    }
}
