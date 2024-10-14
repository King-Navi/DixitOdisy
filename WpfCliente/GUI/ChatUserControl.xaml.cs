﻿using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ChatUserControl.xaml
    /// </summary>
    public partial class ChatUserControl : UserControl , IServicioChatMotorCallback , IActualizacionUI
    {
        public ChatUserControl()
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            InitializeComponent();
            gridChat.Visibility = Visibility.Collapsed;
        }

        private void ClicButtonAbrirChat(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Visible;
            buttonAbrirChat.Visibility = Visibility.Collapsed;
        }

        private async void ClicButtonEnviar(object sender, RoutedEventArgs e)
        {
            try
            {
                await Conexion.ChatMotorCliente.EnviarMensajeAsync(Singleton.Instance.IdChat, new ChatMensaje
                {
                    Mensaje = textBoxEnviarMensaje.Text,
                    HoraFecha = DateTime.Now,
                    Nombre = Singleton.Instance.NombreUsuario
                });
            }
            catch (Exception excepcion)
            {
                //TODO:Manejar execpcion
            }
        }

        private void ClicButtonCerrarChar(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
            buttonAbrirChat.Visibility = Visibility.Visible;
        }

        public void RecibirMensajeCliente(ChatMensaje mensaje)
        {
            //TODO: Realizar el diseño grafico del chat, si es que tiene colores logica para colocarlos
            textBoxReceptorMensaje.Text += mensaje.HoraFecha +" | " + mensaje.Nombre + " dice: " +mensaje.Mensaje;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //TODO: Pedirle a unaay los .resx
        }

        private void CerrarControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
        }
    }
}
