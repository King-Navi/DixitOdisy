using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            InitializeComponent();
            Unloaded += CerrandoUserControl;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
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

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            try
            {
                CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }

        }



        private void CargadoUserControl(object sender, RoutedEventArgs e)
        {
            try
            {
                Imagen.GuardarBitmapImageABytes(SingletonCliente.Instance.FotoJugador);
                BitmapImage bitmapImagen = Imagen.ConvertirBytesABitmapImage(Imagen.ObtenerFotoGlobal());
                ActualizarUI();
                imagenPerfil.Source = bitmapImagen;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }
    }
}
