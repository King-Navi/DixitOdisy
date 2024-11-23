using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class UsuarioUserControl : UserControl
    {

        public UsuarioUserControl()
        {
            InitializeComponent();
            FondoColorAleatorio();
            UsuarioRemitenteEsIgualADestino();
        }

        private void ClicButtonEnviarSolicitud(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is Usuario usuario && !UsuarioRemitenteEsIgualADestino())
                {
                    Usuario usuarioActual = new Usuario
                    {
                        IdUsuario = SingletonCliente.Instance.IdUsuario,
                    Nombre = SingletonCliente.Instance.NombreUsuario
                    };
                    Conexion.Amigos.EnviarSolicitudAmistad(usuarioActual, usuario.Nombre);
                    buttonEnviarSolicitud.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
            }
        }

        private bool UsuarioRemitenteEsIgualADestino()
        {
            try
            {
                if (DataContext is Usuario usuario && SingletonCliente.Instance.NombreUsuario.Equals(usuario.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    buttonEnviarSolicitud.Visibility = Visibility.Collapsed;
                    return true;
                }
            }
            catch (Exception)
            {
                buttonEnviarSolicitud.Visibility = Visibility.Collapsed;
            }
            return false;

        }

        private void FondoColorAleatorio()
        {
            this.Background = Utilidades.GetColorAleatorio();
        }
    }
}