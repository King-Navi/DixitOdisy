using DAOLibreria.ModeloBD;
using System.Collections.Generic;

namespace DAOLibreria.Interfaces
{
    public interface IAmistadDAO
    {
        /// <summary>
        /// Recupera una lista de amigos de un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>Una lista de usuarios que son amigos del usuario especificado.</returns>
        List<Usuario> RecuperarListaAmigos(int idUsuario);

        /// <summary>
        /// Verifica si dos usuarios son amigos.
        /// </summary>
        /// <param name="idUsuarioRemitente">El ID del primer usuario.</param>
        /// <param name="idUsuarioDestinatario">El ID del segundo usuario.</param>
        /// <returns>`true` si los usuarios son amigos; `false` en caso contrario.</returns>
        bool SonAmigos(int idUsuarioRemitente, int idUsuarioDestinatario);

        /// <summary>
        /// Elimina la relación de amistad entre dos usuarios.
        /// </summary>
        /// <param name="idUsuarioMayor">El ID del usuario con el ID mayor.</param>
        /// <param name="idUsuarioMenor">El ID del usuario con el ID menor.</param>
        /// <returns>`true` si la relación de amistad fue eliminada con éxito; `false` en caso contrario.</returns>
        bool EliminarAmigo(int idUsuarioMayor, int idUsuarioMenor);
    }
}
