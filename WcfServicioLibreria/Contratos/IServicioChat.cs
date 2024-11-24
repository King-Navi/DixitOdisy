using System;
using System.ServiceModel;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioChat
    {
        /// <summary>
        /// Crea un nuevo chat con un identificador único especificado.
        /// </summary>
        /// <param name="idChat">El identificador único del chat a crear.</param>
        /// <returns>True si el chat se creó correctamente; False en caso contrario.</returns>
        [OperationContract]
        bool CrearChat(string idChat);
        /// <summary>
        /// Borra un chat existente. Este método se llama en respuesta a un evento, por ejemplo, cuando no quedan usuarios en el chat.
        /// </summary>
        /// <param name="sender">El objeto que envía la solicitud de borrado, generalmente el chat que se va a eliminar.</param>
        /// <param name="e">Argumentos del evento relacionados con la solicitud de borrado.</param>
        void BorrarChat(object sender, EventArgs e);
    }
}
