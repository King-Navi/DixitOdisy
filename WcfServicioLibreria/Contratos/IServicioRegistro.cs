using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        [OperationContract]
        int RegistrarUsuario(string usuario, string contrasenia);
    }
}
