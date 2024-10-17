using System.Net.NetworkInformation;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface  IServicioUsuario
    {
        //TODO: Colcocar lo que le corresponde a la logica del usuario
        void DesconectarUsuario(int idUsuario);
        bool YaIniciadoSesion(string nombreUsuario);
        [OperationContract]
        bool Ping();

        [OperationContract]
        Usuario ValidarCredenciales(string gamertag, string contrasenia);
    }


}
