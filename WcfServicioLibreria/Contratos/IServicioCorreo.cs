using System.ServiceModel;
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
        bool VerificarCorreo(Usuario usuario);

        /// <summary>
        /// Genera un código único para procesos de verificación o recuperación.
        /// </summary>
        /// <remarks>
        /// Este método no está decorado con OperationContract porque está destinado a ser utilizado internamente
        /// o en implementaciones específicas de los métodos del servicio que requieren un código.
        /// </remarks>
        /// <returns>Un código alfanumérico generado aleatoriamente.</returns>
        string GenerarCodigo();

        /// <summary>
        /// Verifica si el código proporcionado para alguna operación de verificación (como cambio de contraseña o verificación de correo) es correcto.
        /// </summary>
        /// <param name="codigo">El código que se debe verificar.</param>
        /// <returns>True si el código es correcto y válido para la operación; False en caso contrario.</returns>
        [OperationContract]
        bool VerificarCodigo(string codigo);

        /// <summary>
        /// Verifica el correo electrónico de un usuario utilizando su gamertag asociado.
        /// </summary>
        /// <param name="usuario">El usuario cuyo correo electrónico se va a verificar usando su gamertag.</param>
        /// <returns>True si el correo electrónico asociado al gamertag es válido; False en caso contrario.</returns>
        [OperationContract]
        bool VerificarCorreoConGamertag(Usuario usuario);

    }
}
