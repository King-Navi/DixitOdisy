using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioAmistad
    {
        [OperationContract]
        int AgregarAmigo(Amigo amigo);
        [OperationContract]
        int ActualizarAmigo(string nombreRemitente, string nombreDestinatario, string peticionEstado);
        [OperationContract]
        int BorrarAmigo(Amigo amigo);
        [OperationContract]
        int ValidarPeticionAmistad(string nombreRemitente, string nombreDestinatario, string peticionEstado);
    }
}
