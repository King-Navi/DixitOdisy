using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface  IServicioUsuario
    {
        [OperationContract]
        bool EditarUsuario(Usuario usuarioEditado);
        [OperationContract]
        [FaultContract(typeof(UsuarioFalla))]
        bool YaIniciadoSesion(string nombreUsuario);
        [OperationContract]
        bool Ping();
        [OperationContract]
        [ServiceKnownType (typeof(Task))]
        [ServiceKnownType (typeof(Task<bool>))]
        Task<bool> PingBDAsync();
        [OperationContract]
        Usuario ValidarCredenciales(string gamertag, string contrasenia);
        [OperationContract]
        bool EditarContraseniaUsuario(string gamertag, string nuevoHashContrasenia);
    }


}
