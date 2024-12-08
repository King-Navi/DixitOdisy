using ChatGPTLibreria;
using ChatGPTLibreria.ModelosJSON;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class GeneradorImagen_Prueba
    {
        private const string EXTENSION_PUNTO_JPG = ".jpg";
        private string rutaImagenes = Rutas.CalcularRutaImagenes(TematicaPartida.Mixta);

        [TestMethod]
        public async Task GenerarImagenDesdeChatGPTAsync_RespuestaValida_RetornaImagenRespuesta()
        {
            var rutaDestino = Path.Combine(rutaImagenes, $"{Guid.NewGuid()}{EXTENSION_PUNTO_JPG}");
            Console.WriteLine(rutaDestino);
            string tematica = "Naturaleza";
            var generadorImagen = new GeneradorImagen();
            var resultado = await generadorImagen.GenerarImagenDesdeChatGPTAsync(tematica);
            var imagenBytes = Convert.FromBase64String(resultado.ImagenDatosList[0].Base64Imagen);
            Assert.IsNotNull(resultado);
            await generadorImagen.GuardarImagenAsync(imagenBytes, rutaDestino);
        }

    }
}
