using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChatMotor
    {
        bool IServicioChatMotor.AgregarUsuarioChat(string idChat, string nombreUsuario)
        {
            try
            {
                bool existeSala = chatDiccionario.TryGetValue(idChat, out Chat chat);
                if (chat is MultiChat multiChat && existeSala)
                {

                    foreach (var nombreJugadorEnSala in multiChat.ObtenerNombresJugadoresChat())
                    {
                        if (nombreJugadorEnSala.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                    IChatCallback contexto = contextoOperacion.GetCallbackChannel<IChatCallback>();
                        return multiChat.AgregarJugadorChat(nombreUsuario, contexto);
                    
                }
               

            }
            catch (Exception)
            {
            }
            return false;
        }


        void IServicioChatMotor.EnviarMensaje(string idChat, ChatMensaje mensaje)
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
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }
    }
}
