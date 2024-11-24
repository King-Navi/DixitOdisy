using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IUsuarioSesionCallback))]
    public interface IServicioUsuarioSesion
    {
        /// <summary>
        /// Solicita la verificación del estado de la sesión de un usuario específico. 
        /// Este método es unidireccional y no espera una respuesta directa al llamador.
        /// </summary>
        /// <param name="usuario">El usuario cuyo estado de sesión se desea verificar.</param>
        [OperationContract(IsOneWay = true)]
        void ObtenerSessionJugador(Usuario usuario);
    }
    [ServiceContract]
    public interface IUsuarioSesionCallback
    {
        /// <summary>
        /// Recibe la notificación sobre el estado de la sesión de un usuario.
        /// </summary>
        /// <param name="esSesionAbierta">Indica si la sesión del usuario está actualmente abierta (true) o cerrada (false).</param>
        [OperationContract]
        void ObtenerSessionJugadorCallback(bool esSesionAbierta);
    }

}
