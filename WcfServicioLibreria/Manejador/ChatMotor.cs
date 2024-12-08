using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChatMotor
    {
        public async Task<bool> AgregarUsuarioChatAsync(string idChat, string nombreUsuario)
        {
            try
            {
                bool existeSala = chatDiccionario.TryGetValue(idChat, out Chat chat);
                if (chat is MultiChat multiChat && existeSala)
                {
                    IChatCallback contexto = contextoOperacion.GetCallbackChannel<IChatCallback>();
                    return await multiChat.AgregarJugadorChatAsync(nombreUsuario, contexto);

                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public void EnviarMensaje(string idChat, ChatMensaje mensaje)
        {
            try
            {
                chatDiccionario.TryGetValue(idChat, out Chat chat);
                lock (chat)
                {
                    MultiChat multiChat = (MultiChat)chat;
                    multiChat.EnviarMensajeTodos(mensaje);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }
    }
}
