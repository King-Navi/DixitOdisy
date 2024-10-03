using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChatMotor , IServicioChat
    {
        bool IServicioChatMotor.AgregarUsuarioChat(string idChat, string nombreUsuario)
        {
            try
            {
                bool existeSala = chatDiccionario.TryGetValue(idChat,out Chat chat);
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
                bool existeSala = chatDiccionario.TryAdd(idChat, new MultiChat(idChat));
                if (!existeSala)
                {
                }
                else
                {
                    resultado = true;
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

        bool IServicioChat.EliminarChat()
        {
            throw new NotImplementedException();
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
        bool IServicioChatMotor.EstadoJugador(string nombreUsuario)
        {
            throw new NotImplementedException();
        }

        List<ChatMensaje> IServicioChatMotor.GetMessageHistory()
        {
            throw new NotImplementedException();
        }
    }
}
