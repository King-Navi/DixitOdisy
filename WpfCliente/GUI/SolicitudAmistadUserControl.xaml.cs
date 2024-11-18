﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SolicitudAmistadUserControl.xaml
    /// </summary>
    public partial class SolicitudAmistadUserControl : UserControl
    {
        SolicitudAmistad solicitudAmistadActual;
        public SolicitudAmistadUserControl()
        {
            InitializeComponent();
            SetFondoColorAleatorio();
            DataContextChanged += SolicitudAmistadUserControl_DataContextChanged;
        }

        private void SolicitudAmistadUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is SolicitudAmistad solicitud)
            {
                labelNombreAmigo.Content = solicitud.Remitente.Nombre;
                imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(solicitud.Remitente.FotoUsuario);
            }
        }

        private void SetFondoColorAleatorio()
        {
            this.Background = Utilidades.GetColorAleatorio();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            _ = AceptarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> AceptarSolicitud(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.AceptarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);
                return resultado;
            }
            catch (Exception excepcion)
            {
                //TODO manejar excepcion
                VentanasEmergentes.CrearVentanaEmergenteCargarDatosAmigosFalla(this);
                return false;
            }
        }

        private void Rechazar_Click(object sender, RoutedEventArgs e)
        {
            _ = _ = RechazarSolicitud(solicitudAmistadActual);
        }

        private async Task<bool> RechazarSolicitud(SolicitudAmistad solicitud)
        {
            Window window = Window.GetWindow(this);
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                var resultado = Conexion.Amigos.RechazarSolicitudAmistad(solicitudAmistadActual.Remitente.IdUsuario, Singleton.Instance.IdUsuario);
                return resultado;
            }
            catch (Exception excepcion)
            {
                //TODO manejar excepcion
                VentanasEmergentes.CrearVentanaEmergenteCargarDatosAmigosFalla(this);
                return false;
            }
        }

        private void HabilitarBotones(bool habilitar)
        {

        }
    }
}
