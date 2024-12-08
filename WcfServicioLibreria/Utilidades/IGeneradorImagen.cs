using ChatGPTLibreria.ModelosJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public interface IGeneradorImagen
    {
        Task GuardarImagenAsync(byte[] imagenBytes, string rutaDestino);
        Task<ImagenRespuesta64JSON> GenerarImagenDesdeChatGPTAsync(string tematica);
    }
}
