using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface  IServicioUsuario
    {
        /// <summary>
        /// Edita y actualiza la información de un usuario en el sistema.
        /// </summary>
        /// <param name="usuarioEditado">El usuario con la información actualizada.</param>
        /// <returns>True si la información del usuario se actualizó correctamente; False en caso contrario.</returns>
        [OperationContract]
        bool EditarUsuario(Usuario usuarioEditado);

        /// <summary>
        /// Verifica si un usuario ya ha iniciado sesión en el sistema.
        /// </summary>
        /// <param name="nombreUsuario">El nombre del usuario a verificar.</param>
        /// <returns>True si el usuario ya ha iniciado sesión; False en caso contrario.</returns>
        /// <exception cref="UsuarioFalla">Se lanza si ocurre un error al verificar el estado de inicio de sesión del usuario.</exception>
        [OperationContract]
        [FaultContract(typeof(UsuarioFalla))]
        bool YaIniciadoSesion(string nombreUsuario);

        /// <summary>
        /// Realiza un ping al servicio para verificar su disponibilidad.
        /// </summary>
        /// <returns>True si el servicio responde correctamente; False en caso contrario.</returns>
        [OperationContract]
        bool Ping();

        /// <summary>
        /// Realiza un ping asincrónico a la base de datos para verificar su conectividad y disponibilidad.
        /// </summary>
        /// <returns>Una tarea que devuelve True si la base de datos responde correctamente; False en caso contrario.</returns>
        [OperationContract]
        [ServiceKnownType(typeof(Task))]
        [ServiceKnownType(typeof(Task<bool>))]
        Task<bool> PingBDAsync();

        /// <summary>
        /// Valida las credenciales de un usuario para permitir el acceso al sistema.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <param name="contrasenia">La contraseña del usuario.</param>
        /// <returns>El usuario validado si las credenciales son correctas; null si las credenciales son incorrectas.</returns>
        [OperationContract]
        Usuario ValidarCredenciales(string gamertag, string contrasenia);

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario cuya contraseña se actualizará.</param>
        /// <param name="nuevoHashContrasenia">El nuevo hash de la contraseña para el usuario.</param>
        /// <returns>True si la contraseña se actualizó correctamente; False en caso contrario.</returns>
        [OperationContract]
        bool EditarContraseniaUsuario(string gamertag, string nuevoHashContrasenia);
    }


}
