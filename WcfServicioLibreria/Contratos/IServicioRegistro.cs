using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        [OperationContract]
        void RegistrarUsuario(String usuario, String contrasenia);
    }
}
