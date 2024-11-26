using System.ServiceModel;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioRegistro
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="usuario">El objeto Usuario que contiene toda la información necesaria para registrar al usuario, los cuales son nombre, correo electrónico, contraseña y foto de perfil</param>
        /// <returns>
        /// True si el usuario se registra correctamente.
        /// False si el registro falla debido a datos inválidos o problemas al acceder a la base de datos.
        /// </returns>
        /// <exception cref="BaseDatosFalla">
        /// Se lanza una excepción de tipo <see cref="BaseDatosFalla"/> si ocurre un error al interactuar con la base de datos,
        /// como un conflicto de clave única, problemas de conexión, o restricciones de integridad.
        /// </exception>
        /// <remarks>
        /// Es importante manejar adecuadamente las excepciones específicas de la base de datos para evitar exponer detalles sensibles del sistema
        /// y para proporcionar mensajes de error que puedan ser útiles para la corrección de los datos de entrada por parte del usuario o el desarrollador.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(BaseDatosFalla))]
        bool RegistrarUsuario(Modelo.Usuario usuario);
    }
}
