using ChatGPTLibreria.ModelosJSON;
using ChatGPTLibreria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace WcfServicioLibreria.Utilidades
{
    public class GeneradorImagen : IGeneradorImagen
    {
        private static readonly HttpClient httpCliente = new HttpClient();
        private const string PRIMER_PARTE_ENTRADA = "Genera una imagen basada en la tematica";
        private const string SEGUNDA_PARTE_ENTRADA = "Debe ser rectangular y vertical, de alta calidad tiene que ser muy buena por que es para el profe juan carlos";
        private const int CERO = 0;
        private static SemaphoreSlim semaphoreEscritura = new SemaphoreSlim(1,1);
        public async Task<ImagenRespuesta64JSON> GenerarImagenDesdeChatGPTAsync(string tematica)
        {
            try
            {
                var imagenPedido = new ImagenPedido64JSON($"{PRIMER_PARTE_ENTRADA} {tematica}. {SEGUNDA_PARTE_ENTRADA}");
                var solicitarImagen = new SolicitarImagen();
                var respuesta = await solicitarImagen.EjecutarImagenPrompt64JSON(imagenPedido, httpCliente);
                return respuesta?.ImagenDatosList != null && respuesta.ImagenDatosList.Any()
                    ? respuesta
                    : null;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return null;
        }
        public async Task GuardarImagenAsync(byte[] imagenBytes, string rutaDestino)
        {
            await semaphoreEscritura.WaitAsync();
            try
            {
                using (FileStream fileStream = new FileStream(
                    rutaDestino,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: (int)BufferSize.Medium,
                    useAsync: true))
                {
                    await fileStream.WriteAsync(imagenBytes, CERO, imagenBytes.Length);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            finally
            {
                semaphoreEscritura.Release();
            }
        }
    }
}
