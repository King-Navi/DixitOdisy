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

namespace WcfServicioLibreria.Modelo
{
    public class ManejadorImagen : IManejadorImagen
    {

        private const int LIMITE_CARTAS_MINIMO = 0;
        private const string PRIMER_PARTE_ENTRADA = "Genera una imagen basada en la tematica";
        private const string SEGUNDA_PARTE_ENTRADA = "Debe ser rectangular y vertical, de alta calidad tiene que ser muy buena por que es para el profe juan carlos";
        private const string EXTENSION_PUNTO_JPG = ".jpg";
        private IMediadorImagen mediadorImagen;
        private readonly TematicaPartida tematica;
        private IEscribirDisco escritor;
        private ILectorDisco lectorDisco;
        private readonly SemaphoreSlim semaphoreLectura = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim semaphoreEnvioGrupo = new SemaphoreSlim(1, 1);
        private static readonly HttpClient httpCliente = new HttpClient();
        private readonly string rutaImagenes;
        private List<string> listaActualElegida;
        public ManejadorImagen(IEscribirDisco _escritor, IMediadorImagen _mediadorImagen, TematicaPartida _tematica, ILectorDisco _lectorDisco , List<string> rutasImagenesElegias)
        {
            escritor = _escritor;
            mediadorImagen = _mediadorImagen;
            tematica = _tematica;
            rutaImagenes = Rutas.CalcularRutaImagenes(tematica);
            lectorDisco = _lectorDisco;
            listaActualElegida = rutasImagenesElegias;
        }

        #region Imagenes

        public async Task<bool> SolicitarImagenMazoAsync(LecturaTrabajo lecturaTrabajo)
        {
            int cartasRestantes = mediadorImagen.ObtenerCartasRestantes();
            try
            {
                if (cartasRestantes <= LIMITE_CARTAS_MINIMO)
                {
                     return await SolicitarImagenChatGPTAsync(lecturaTrabajo.CallbackImagenesMazo);
                }
                else
                {
                    return await LeerImagenMazoDiscoAsync(lecturaTrabajo);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return false;
        }

        private async Task<bool> LeerImagenMazoDiscoAsync(LecturaTrabajo lecturaTrabajo)
        {
            await semaphoreLectura.WaitAsync();

            try
            {
                return await lectorDisco.ProcesarColaLecturaEnvio(lecturaTrabajo);
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

        private async Task<bool> SolicitarImagenChatGPTAsync(IImagenMazoCallback callback)
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
                callback?.RecibirImagenCallback(imagenCarta);
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
                using (MemoryStream memoria = new MemoryStream(imagenBytes))
                {
                    escritor.EncolarEscritura(memoria, rutaDestino);
                }
                return new ImagenCarta
                {
                    ImagenStream = imagenBytes,
                    IdImagen = Path.GetFileNameWithoutExtension(rutaDestino)
                };

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
                return null;
            }
        }
        
        public async Task<bool> SolicitarTableroCartasAsync(IImagenesTableroCallback callback)
        {
            try
            {
                if (listaActualElegida == null || listaActualElegida.Count == 0)
                {
                    return false;
                }
                List <String> resultadoRutasCompletas = mediadorImagen.ObtenerRutasPorNombreArchivo(listaActualElegida);
                LecturaTrabajo lecturaTrabajo = new LecturaTrabajo(resultadoRutasCompletas, callback);
                return await lectorDisco.ProcesarColaLecturaEnvio(lecturaTrabajo);
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
            semaphoreEnvioGrupo.Release();
            return false;
        }

        #endregion Imagenes
    }
}

