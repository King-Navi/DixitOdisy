using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IInvitacionPartidaCallback))]
    public interface IServicioInvitacionPartida
    {
        [OperationContract]
        bool EnviarInvitacion(string gamertagEmisor, string codigoSala, string gamertagReceptor);
    }

    [ServiceContract]
    public interface IInvitacionPartidaCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecibirInvitacion(InvitacionPartida invitacion);
    }
}
