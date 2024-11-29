using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChat
    {
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
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
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
    }

  
}
