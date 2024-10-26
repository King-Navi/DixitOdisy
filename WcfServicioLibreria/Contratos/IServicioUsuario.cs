using System.Net.NetworkInformation;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface  IServicioUsuario
    {
        [OperationContract]
        bool EditarUsuario(Usuario usuarioEditado);
        void DesconectarUsuario(int idUsuario);
        [OperationContract]
        [FaultContract(typeof(UsuarioFalla))]
        bool YaIniciadoSesion(string nombreUsuario);
        [OperationContract]
        bool Ping();
        [OperationContract]
        Usuario ValidarCredenciales(string gamertag, string contrasenia);
    }


}
