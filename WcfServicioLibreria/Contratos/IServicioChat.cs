using System;
using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioChat
    {
        [OperationContract]
        bool CrearChat(string idChat);
        void BorrarChat(object sender, EventArgs e);
    }
}
