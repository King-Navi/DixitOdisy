using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioSala
    {
        [OperationContract]
        string CrearSala(string nombreUsuarioAnfitrion);
        [OperationContract]
        bool BorrarSala(string idSala);
        [OperationContract]
        bool ValidarSala(string idSala);
    }
}
