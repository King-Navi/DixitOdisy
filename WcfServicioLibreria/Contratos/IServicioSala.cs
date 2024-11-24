using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioSala
    {
        /// <summary>
        /// Crea una nueva sala asignada a un usuario anfitrión.
        /// </summary>
        /// <param name="nombreUsuarioAnfitrion">El nombre del usuario que actuará como anfitrión de la sala. Este usuario tendrá ciertos privilegios de gestión sobre la sala.</param>
        /// <returns>El identificador único de la sala creada, que puede ser utilizado para acceder a la sala o realizar operaciones adicionales sobre ella.</returns>
        [OperationContract]
        string CrearSala(string nombreUsuarioAnfitrion);

        /// <summary>
        /// Valida si una sala con un identificador específico existe y está en un estado válido para ser utilizada.
        /// </summary>
        /// <param name="idSala">El identificador de la sala que se necesita validar.</param>
        /// <returns>True si la sala existe y es accesible; False en caso contrario.</returns>
        [OperationContract]
        bool ValidarSala(string idSala);
    }
}
