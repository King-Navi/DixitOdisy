﻿using System;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for UniserSalaModalWindow.xaml
    /// </summary>
    public partial class UnirseSalaModalWindow : IActualizacionUI
    {
        public string ValorIngresado { get; private set; }

        public UnirseSalaModalWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }


        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelIngresarCodigoSala.Content = Properties.Idioma.labelIngresarCodigoSala;
            buttonAceptar.Content = Properties.Idioma.buttonAceptar;
            this.Title = Properties.Idioma.tituloIngresarCodigoSala;
        }

        private async void ClicButtonAceptar(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            ValorIngresado = textBoxCodigoSala.Text.ToUpper();
            
            if(!string.IsNullOrWhiteSpace(ValorIngresado))
            {

                DialogResult = true;
                this.Close();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
            }

        }

        private void HabilitarBotones(bool esHabilitado)
        {
            buttonAceptar.IsEnabled = esHabilitado;
        }
    }
}
