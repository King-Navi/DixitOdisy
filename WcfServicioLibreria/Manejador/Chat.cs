﻿using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioChat
    {
        public bool CrearChat(string idChat)
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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (OverflowException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return resultado;
        }

        public void BorrarChat(object sender, System.EventArgs e)
        {
            if (sender is MultiChat chat)
            {
                MultiChatVacioEventArgs evento = e as MultiChatVacioEventArgs;
                chat.EliminarChatManejadorEvento -= BorrarChat;
                salasDiccionario.TryRemove(evento.Chat.IdChat, out _);
            }
        }
    }


}
