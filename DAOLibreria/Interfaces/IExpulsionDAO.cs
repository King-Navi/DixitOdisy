namespace DAOLibreria.Interfaces
{
    public interface IExpulsionDAO
    {
        /// <summary>
        /// Verifica si un usuario tiene más de diez expulsiones sin penalizar.
        /// </summary>
        /// <param name="idUsuarioCuenta">El ID de la cuenta del usuario.</param>
        /// <returns>`true` si el usuario tiene diez o más expulsiones sin penalizar; `false` en caso contrario.</returns>
        bool TieneMasDeDiezExpulsionesSinPenalizar(int idUsuarioCuenta);

        /// <summary>
        /// Cambia el estado de las expulsiones de un usuario a penalizadas.
        /// </summary>
        /// <param name="idUsuarioCuenta">El ID de la cuenta del usuario.</param>
        /// <returns>`true` si las expulsiones fueron penalizadas con éxito; `false` en caso contrario.</returns>
        bool CambiarExpulsionesAFueronPenalizadas(int idUsuarioCuenta);

        /// <summary>
        /// Crea un nuevo registro de expulsión para un usuario.
        /// </summary>
        /// <param name="idUsuarioCuenta">El ID de la cuenta del usuario.</param>
        /// <param name="motivo">El motivo de la expulsión.</param>
        /// <param name="esHacker">Indica si el usuario fue expulsado por ser hacker.</param>
        /// <returns>`true` si el registro fue creado con éxito; `false` en caso contrario.</returns>
        bool CrearRegistroExpulsion(int idUsuarioCuenta, string motivo, bool esHacker);
    }
}
