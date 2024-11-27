using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioCorreo
    {
        /// <summary>
        /// Verifica si el correo electrónico asociado a un usuario es válido y se encuentra en buen estado.
        /// </summary>
        /// <param name="usuario">El usuario cuyo correo electrónico se va a verificar.</param>
        /// <returns>True si el correo electrónico es válido; False en caso contrario.</returns>
        [OperationContract]
        Task<bool> VerificarCorreoAsync(Usuario usuario);

        /// <summary>
        /// Verifica si el código proporcionado para alguna operación de verificación (como cambio de contraseña o verificación de correo) es correcto.
        /// </summary>
        /// <param name="codigo">El código que se debe verificar.</param>
        /// <returns>True si el código es correcto y válido para la operación; False en caso contrario.</returns>
        [OperationContract]
        bool VerificarCodigo(string codigo, string correoUsuario);

        /// <summary>
        /// Verifica el correo electrónico de un usuario utilizando su gamertag asociado.
        /// </summary>
        /// <param name="usuario">El usuario cuyo correo electrónico se va a verificar usando su gamertag.</param>
        /// <returns>True si el correo electrónico asociado al gamertag es válido; False en caso contrario.</returns>
        [OperationContract]
        bool VerificarCorreoConGamertag(Usuario usuario);

    }
}
