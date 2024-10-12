using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
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
            labelSelecionUsuario.Content = Properties.Idioma.labelSeleccionarFotoPerfil;
            labelRepitaContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia; 
            labelUsuario.Content = Properties.Idioma.labelUsuario;
            labelCorreo.Content = Properties.Idioma.labelCorreoE;
            labelRegistro.Content = Properties.Idioma.tituloRegistroUsuario;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private async void ButtonClicRegistrarUsuario(object sender, RoutedEventArgs e)
        {
            //TODO: Validar campos no vacios
            string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(passwordBoxContrasenia.Password))).Replace("-", "");
            var manejadorConexion = new ServicioManejador<ServicioRegistroClient>();
            Task<bool> resultado = manejadorConexion.EjecutarServicioAsync(async servicio =>
            {
                return await servicio.RegistrarUsuarioAsync(Singleton.Instance.NombreUsuario, contraseniaHash);
            });
            if (await resultado)
            {
                MessageBox.Show("Exito en la prueba");
            }else
            {
                MessageBox.Show("Fallo la prueba");

            }
        }
    }
}
