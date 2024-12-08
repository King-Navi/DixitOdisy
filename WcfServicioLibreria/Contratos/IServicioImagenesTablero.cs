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
        [OperationContract]
        Task<bool> MostrarImagenesTablero(string idPartida);
    }
    public interface IImagenesTableroCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecibirGrupoImagenCallback(List<ImagenCarta> imagen);
    }
}
