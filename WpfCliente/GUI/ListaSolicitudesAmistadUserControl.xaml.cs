﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Security;
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
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para ListaSolicitudesAmistadUserControl.xaml
    /// </summary>
    public partial class ListaSolicitudesAmistadUserControl : UserControl, IActualizacionUI, IHabilitadorBotones
    {
        public ObservableCollection<SolicitudAmistad> Solicitudes { get; set; } = new ObservableCollection<SolicitudAmistad>();
        public ListaSolicitudesAmistadUserControl()
        {
            InitializeComponent();
            DataContext = this;
            _ = CargarSolicitudes();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }

        private async Task<bool> CargarSolicitudes()
        {
            try
            {
                Usuario usuarioActual = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };

                Window window = Window.GetWindow(this);

                bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, window);
                if (!conexionExitosa)
                {
                    return false;
                }

                try
                {
                    var listaSolicitudes = Conexion.Amigos.ObtenerSolicitudesAmistad(usuarioActual);
                    if (listaSolicitudes == null || listaSolicitudes.Count() == 0)
                    {
                        labelNoHaySolicitudes.Visibility = Visibility.Visible;
                        return false;
                    }
                    Solicitudes.Clear();
                    foreach (var solicitud in listaSolicitudes)
                    {
                        var nuevaSolicitud = new SolicitudAmistad { Remitente = solicitud };
                        Solicitudes.Add(nuevaSolicitud);
                    }
                    
                    return true;
                }
                catch (Exception e)
                {
                    //TODO MANEJAR EL ERROR
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar solicitudes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelSolicitudes.Content = Idioma.labelSolicitudesAmistad;
            labelNoHaySolicitudes.Content = Idioma.labelNoHaySolicitudes;
        }
    }
}
