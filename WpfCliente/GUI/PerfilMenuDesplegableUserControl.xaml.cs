using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class PerfilMenuDesplegableUserControl : UserControl , IActualizacionUI
    {
        public PerfilMenuDesplegableUserControl()
        {
            InitializeComponent();
            Loaded += CargadoUserControl;
            Unloaded += CerrandoUserControl;
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
            if (imagenPerfil == null)
            {
                return;
            }
            try
            {
                CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
                imagenPerfil.Source = null;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }

        }
        public void LimpiarFoto()
        {
            imagenPerfil = null;
        }


        private void CargadoUserControl(object sender, RoutedEventArgs e)
        {
            if (imagenPerfil.Source != null)
            {
                return;
            }
            try
            {
                Imagen.GuardarBitmapImageABytes(SingletonCliente.Instance.FotoJugador);
                BitmapImage bitmapImagen = Imagen.ConvertirBytesABitmapImage(Imagen.ObtenerFotoGlobal());
                CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
                ActualizarUI();
                imagenPerfil.Source = bitmapImagen;
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }
    }
}
