using DAOLibreria.ModeloBD;
using System.Collections.Generic;

namespace DAOLibreria.Interfaces
{
    public interface IUsuarioDAO
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema junto con su cuenta asociada y estadísticas iniciales.
        /// </summary>
        bool RegistrarNuevoUsuario(Usuario usuario, UsuarioCuenta usuarioCuenta);

        /// <summary>
        /// Edita la información de un usuario.
        /// </summary>
        bool EditarUsuario(UsuarioPerfilDTO usuarioEditado);

        /// <summary>
        /// Valida las credenciales del usuario.
        /// </summary>
        UsuarioPerfilDTO ValidarCredenciales(string gamertag, string contrasenia);

        /// <summary>
        /// Obtiene un usuario por su nombre (gamertag).
        /// </summary>
        Usuario ObtenerUsuarioPorNombre(string gamertag);

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        Usuario ObtenerUsuarioPorId(int idUsuario);

        /// <summary>
        /// Obtiene el ID de un usuario dado su nombre (gamertag).
        /// </summary>
        int ObtenerIdPorNombre(string gamertag);

        /// <summary>
        /// Obtiene una lista de usuarios por una lista de nombres (gamertags).
        /// </summary>
        List<Usuario> ObtenerListaUsuariosPorNombres(List<string> nombres);

        /// <summary>
        /// Verifica si un nombre de usuario (gamertag) es único.
        /// </summary>
        void VerificarNombreUnico(string gamertag);

        /// <summary>
        /// Coloca la última conexión del usuario.
        /// </summary>
        bool ColocarUltimaConexion(int idUsuario);
    }
}
