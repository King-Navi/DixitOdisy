using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IServicioChat
    {
        [OperationContract]
        ChatUsuario ConectarUsuario(string usuario);
        [OperationContract]
        List<ChatUsuario> ObtenerUsuariosConectados();
        [OperationContract]
        void EnviarMensaje(ChatMensaje nuevoMensaje);
        [OperationContract]
        List<ChatMensaje> GetMensajeNuevo(ChatUsuario usuario);
        [OperationContract]
        void DesconectarUsuario(ChatUsuario usuario);

    }
}
