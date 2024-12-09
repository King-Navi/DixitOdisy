using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IImagenesTableroCallback))]
    public interface IServicioImagenesTablero
    {
        /// <summary>
        /// Muestra las imágenes asociadas al tablero de una partida específica.
        /// </summary>
        /// <param name="idPartida">Identificador único de la partida.</param>
        /// <returns>Una tarea que indica si la operación fue exitosa.</returns>
        [OperationContract]
        Task<bool> MostrarImagenesTablero(string idPartida);
    }
    public interface IImagenesTableroCallback
    {
        /// <summary>
        /// Recibe un grupo de imágenes como respuesta para mostrarlas en el tablero.
        /// </summary>
        /// <param name="imagen">Lista de objetos que representan las imágenes de las cartas.</param>
        [OperationContract(IsOneWay = true)]
        void RecibirGrupoImagenCallback(List<ImagenCarta> imagen);
    }
}
