using DAOLibreria.ModeloBD;
using System.Collections.Generic;

namespace DAOLibreria.Interfaces
{

    public interface ISolicitudAmistadDAO
    {
        /// <summary>
        /// Guarda una solicitud de amistad entre dos usuarios.
        /// </summary>
        /// <param name="idUsuarioRemitente">ID del usuario que envía la solicitud.</param>
        /// <param name="idUsuarioDestinatario">ID del usuario que recibe la solicitud.</param>
        /// <returns>`true` si la solicitud fue guardada con éxito; `false` en caso contrario.</returns>
        bool GuardarSolicitudAmistad(int idUsuarioRemitente, int idUsuarioDestinatario);

        /// <summary>
        /// Verifica si ya existe una solicitud de amistad entre dos usuarios.
        /// </summary>
        /// <param name="idRemitente">ID del usuario remitente.</param>
        /// <param name="idDestinatario">ID del usuario destinatario.</param>
        /// <returns>`true` si existe una solicitud de amistad; `false` en caso contrario.</returns>
        bool ExisteSolicitudAmistad(int idRemitente, int idDestinatario);

        /// <summary>
        /// Obtiene una lista de usuarios que han enviado solicitudes de amistad a un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario destinatario.</param>
        /// <returns>Lista de usuarios que enviaron solicitudes de amistad.</returns>
        List<Usuario> ObtenerSolicitudesAmistad(int idUsuario);

        /// <summary>
        /// Acepta una solicitud de amistad entre dos usuarios.
        /// </summary>
        /// <param name="idRemitente">ID del usuario remitente.</param>
        /// <param name="idDestinatario">ID del usuario destinatario.</param>
        /// <returns>`true` si la solicitud fue aceptada con éxito; `false` en caso contrario.</returns>
        bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario);

        /// <summary>
        /// Rechaza una solicitud de amistad entre dos usuarios.
        /// </summary>
        /// <param name="idRemitente">ID del usuario remitente.</param>
        /// <param name="idDestinatario">ID del usuario destinatario.</param>
        /// <returns>`true` si la solicitud fue rechazada con éxito; `false` en caso contrario.</returns>
        bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario);
    }
}
