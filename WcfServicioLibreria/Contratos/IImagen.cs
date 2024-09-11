using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IImagen
    {
        [OperationContract]
         Byte[] PedirImagen(int ususarioID);
    }
}
