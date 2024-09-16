using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using UtilidadesLibreria;

namespace WpfCliente.UsuarioControl
{
    /// <summary>
    /// Lógica de interacción para RegistrarUsuario.xaml
    /// </summary>
    public partial class RegistrarUsuario : UserControl, IActualizacionUI
    {
        private Boolean esInivtado;
        public RegistrarUsuario()
        {

            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }public RegistrarUsuario(bool esInvitado)
        {
            this.esInivtado = esInvitado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelSelecionUsuario.Content = WpfCliente.Properties.Idioma.labelSeleccionarFotoPerfil;
            labelRepitaContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia; 
            labelUsuario.Content = Properties.Idioma.labelUsuario;
            labelCorreo.Content = Properties.Idioma.labelCorreoE;
            labelRegistro.Content = WpfCliente.Properties.Idioma.tituloRegistroUsuario;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private void buttonClicRegistrarUsuario(object sender, RoutedEventArgs e)
        {
            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(passwordBoxContrasenia.Password))).Replace("-", "");
            int resultado=
            servicio.RegistrarUsuario(textBoxUsuario.Text, contraseniaHash);
            if (resultado < 1)
            {
                Console.WriteLine("error");
            }
        }
    }
}
