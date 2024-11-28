﻿using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class PerfilMenuDesplegableUserControl : UserControl , IActualizacionUI
    {
        public PerfilMenuDesplegableUserControl()
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            ActualizarUI();
            imagenPerfil.Source = SingletonCliente.Instance.FotoJugador;
        }

        private void AbrirEditarVentana(object sender, RoutedEventArgs e)
        {
            SingletonGestorVentana.Instancia.NavegarA(new EditarPerfilPage());
        }

        private void CerrarSesion(object sender, RoutedEventArgs e)
        {
            SingletonGestorVentana.Instancia.NavegarA(new IniciarSesionPage());
        }

        public void ActualizarUI()
        {
            menuItemEditarPerfil.Header = Idioma.menuItemEditarperfil;
            menuItemCerrarSesion.Header = Idioma.buttonCerrarSesion;
            menuItemPerfil.Header = Idioma.menuItemPerfil;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }
    }
}
