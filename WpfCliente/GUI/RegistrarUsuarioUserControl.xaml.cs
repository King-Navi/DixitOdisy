using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
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

        private void ButtonClicRegistrarUsuario(object sender, RoutedEventArgs e)
        {
            //TODO: Realizar caso en el que no hay conexion
            ServidorDescribelo.IServicioRegistro servicio = new ServidorDescribelo.ServicioRegistroClient();
            try
            {
                string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(passwordBoxContrasenia.Password))).Replace("-", "");
                int resultado = servicio.RegistrarUsuario(textBoxUsuario.Text, contraseniaHash);
                if (resultado < 1)
                {
                    //TODO: Manejar el error
                    Console.WriteLine("error");
                }
            }
            catch (Exception)
            {
                //TODO: Manejar error
            }finally
            {
                if (servicio != null)
                {
                    ((ICommunicationObject)servicio).Close();
                }
            }
        }
    }
}
