using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IInvitacionPartidaCallback))]
    public interface IServicioInvitacionPartida
    {
        [OperationContract]
        bool EnviarInvitacion(string gamertagEmisor, string codigoSala, string gamertagReceptor);
        [OperationContract(IsOneWay = true)]
        void AbrirCanalParaInvitaciones(Usuario usuario);
    }

    [ServiceContract]
    public interface IInvitacionPartidaCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecibirInvitacion(InvitacionPartida invitacion);
    }
}
