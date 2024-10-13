using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChatMotor, IServicioChat
    {
        bool IServicioChatMotor.AgregarUsuarioChat(string idChat, string nombreUsuario)
        {
            try
            {
                bool existeSala = chatDiccionario.TryGetValue(idChat, out Chat chat);
                if (existeSala)
                {
                    MultiChat multiChat = (MultiChat)chat;
                    IChatCallback contexto = OperationContext.Current.GetCallbackChannel<IChatCallback>();
                    return multiChat.AgregarJugadorChat(nombreUsuario, contexto);
                }

            }
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
            return false;
        }

        bool IServicioChat.CrearChat(string idChat)
        {
            bool resultado = false;
            try
            {
                MultiChat multiChat = new MultiChat(idChat);
                bool existeSala = chatDiccionario.TryAdd(idChat, multiChat);
                if (existeSala)
                {
                    resultado = true;
                    multiChat.EliminarChatManejadorEvento += BorrarChat;
                }
                else
                {
                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
            return resultado;
        }

        bool IServicioChatMotor.DesconectarUsuarioChat(string usuario)
        {
            throw new NotImplementedException();
        }

        public void BorrarChat(object sender, System.EventArgs e)
        {
            if (sender is MultiChat chat)
            {
                MultiChatVacioEventArgs evento = e as MultiChatVacioEventArgs;
                chat.EliminarChatManejadorEvento -= BorrarChat;
                Console.WriteLine($"El chat con ID {evento.Chat.IdChat} está vacío y será eliminada.");
                salasDiccionario.TryRemove(evento.Chat.IdChat, out _);
            }
        }


        void IServicioChatMotor.EnviarMensaje(string idChat, ChatMensaje mensaje)
        {
            try
            {
                ///Problema de concurrencia granular y revisar riesgo de deadlock
                chatDiccionario.TryGetValue(idChat, out Chat chat);
                lock (chat)
                {
                    MultiChat multiChat = (MultiChat)chat;
                    multiChat.EnviarMensajeTodos(mensaje);
                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
        }
    }
}
