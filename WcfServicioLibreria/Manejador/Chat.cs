using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
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
                    BroadCastChat chatBroadCart = (BroadCastChat)chat;
                    IChatCallback contexto = OperationContext.Current.GetCallbackChannel<IChatCallback>();
                     return chatBroadCart.AgregarJugadorChat(nombreUsuario, contexto);
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
                bool existeSala = chatDiccionario.TryAdd(idChat, new BroadCastChat(idChat));
                if (!existeSala)
                {
                    resultado = false;
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

        void IServicioChatMotor.EnviarMensaje(string nombreUsuario, string mensaje)
        {
            throw new NotImplementedException();
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
