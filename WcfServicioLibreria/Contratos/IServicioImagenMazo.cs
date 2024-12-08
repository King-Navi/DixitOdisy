using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IImagenMazoCallback))]
    public interface IServicioImagenMazo
    {
        [OperationContract]
        Task<bool> SolicitarImagenMazoAsync(string idPartida, int numeroImagenes = 1);
    }

    [ServiceContract]
    public interface IImagenMazoCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecibirImagenCallback(ImagenCarta imagen);        
        [OperationContract(IsOneWay = true)]
        void RecibirVariasImagenCallback(List<ImagenCarta> imagenes);
    }
}
