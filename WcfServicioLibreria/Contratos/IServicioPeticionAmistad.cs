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
        void ObtenerPeticionAmistad(string nombreUsuario);
        [OperationContract]
        [FaultContract(typeof(UsuarioFalla))]
        bool AbrirCanalParaPeticiones(Usuario usuario);
    }
    [ServiceContract]
    public interface IServicioPeticionAmistadCallBack
    {
        [OperationContract]
        Task<bool> ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad);
    }
}
