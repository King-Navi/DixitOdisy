using System;
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
using System.Windows.Shapes;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para CambiarContraseniaWindow.xaml
    /// </summary>
    public partial class CambiarContraseniaWindow : Window
    {
        Usuario usuarioEditado = new Usuario();
        public CambiarContraseniaWindow(string gamertag)
        {
            InitializeComponent();
            usuarioEditado.Nombre = gamertag;
        }

        private void clicButtonAceptar(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos()){
                
                GuardarCambiosUsuario(usuarioEditado);
            }
        }

        private bool ValidarCampos()
        {
            bool isValid = true;
            SetDefaultStyles();

            if (!ValidarCaracteristicasContrasenia())
            {
                isValid = false;
            }

            return isValid;
        }

        private void SetDefaultStyles()
        {
            labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;

            labelContraseniaMinimo.Foreground = Brushes.Red;
            labelContraseniaMaximo.Foreground = Brushes.Red;
            labelContraseniaSimbolos.Foreground = Brushes.Red;
        }

        private bool ValidarCaracteristicasContrasenia()
        {
            bool isValid = true;

            if (textBoxContrasenia.Password.Trim().Length >= 5)
            {
                labelContraseniaMinimo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password.Trim().Length <= 20)
            {
                labelContraseniaMaximo.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (ValidacionesString.IsValidSymbol(textBoxContrasenia.Password))
            {
                labelContraseniaSimbolos.Foreground = Brushes.Green;
            }
            else
            {
                isValid = false;
            }

            if (textBoxContrasenia.Password != textBoxRepetirContrasenia.Password)
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                labelContraseniasNoCoinciden.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        private void GuardarCambiosUsuario(Usuario usuarioEditado)
        {
            usuarioEditado.IdUsuario = Singleton.Instance.IdUsuario;
            usuarioEditado.Nombre = Singleton.Instance.NombreUsuario;

            var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
            bool resultado = manejadorServicio.EjecutarServicio(proxy =>
            {
                return proxy.EditarUsuario(usuarioEditado);
            });

            if (resultado)
            {
                VentanasEmergentes.CrearVentanaEmergenteDatosEditadosExito(this);
                Application.Current.Shutdown();
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloEditarUsuario, Idioma.mensajeUsuarioEditadoFallo, this);
            }

        }
        private void clicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }

}
