using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IServicioPeticionAmistadCallBack))]
    public interface IServicioPeticionAmistad
    {
        [OperationContract(IsOneWay = true)]
        void GetFriendRequest(string nombreUsuario);
    }
    [ServiceContract]
    public interface IServicioPeticionAmistadCallBack
    {
        [OperationContract]
        void GetFriendRequestCallback(List<Amigo> amigos);
    }
}
