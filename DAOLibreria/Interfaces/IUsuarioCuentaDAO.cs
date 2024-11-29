using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Interfaces
{
    public interface IUsuarioCuentaDAO
    {
        /// <summary>
        /// Obtiene el identificador de cuenta asociado a un identificador de usuario específico.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario para el cual se desea obtener el identificador de cuenta asociado.</param>
        /// <returns>
        /// El identificador de la cuenta del usuario si existe; null si no tiene una cuenta asociada
        /// o si el identificador de cuenta es 0. Retorna -1 en caso de cualquier error. En caso de no encontrar usuario con ese id -2
        /// </returns>
        int ObtenerIdUsuarioCuentaPorIdUsuario(int idUsuario);

        /// <summary>
        /// Edita la contraseña de un usuario dado su gamertag.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <param name="nuevoHashContrasenia">El nuevo hash de la contraseña.</param>
        /// <returns>
        /// `true` si la operación fue exitosa, `false` en caso contrario.
        /// </returns>
        bool EditarContraseniaPorGamertag(string gamertag, string nuevoHashContrasenia);

        /// <summary>
        /// Verifica si existe un usuario único con el gamertag y correo especificados.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <param name="correo">El correo del usuario.</param>
        /// <returns>
        /// `true` si existe al menos un usuario con el gamertag y correo especificados; `false` en caso contrario.
        /// </returns>
        bool ExisteUnicoUsuarioConGamertagYCorreo(string gamertag, string correo);
    }
}
