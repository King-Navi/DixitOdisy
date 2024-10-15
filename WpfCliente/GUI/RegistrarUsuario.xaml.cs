using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
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
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para RegistrarUsuario.xaml
    /// </summary>
    public partial class RegistrarUsuario : Window, IActualizacionUI
    {
        private Boolean esInivtado;
        private string[] imagenesPerfil = {
            "/Recursos/pfp1.png",
            "/Recursos/pfp2.png",
            "/Recursos/pfp3.png",
            "/Recursos/pfp4.png",
            "/Recursos/pfp5.png"
        };
        private int contadorImagen = 0;

        public RegistrarUsuario()
        {

            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
        }
        public RegistrarUsuario(bool esInvitado)
        {
            this.esInivtado = esInvitado;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelRegistro.Content = Properties.Idioma.tituloRegistroUsuario;
            labelFotoPerfil.Content = Properties.Idioma.labelSeleccionarFotoPerfil;
            labelRepetirContrasenia.Content = Properties.Idioma.labelRepitaContraseña;
            labelContrasenia.Content = Properties.Idioma.labelContrasenia;
            labelGamertag.Content = Properties.Idioma.labelUsuario;
            labelCorreo.Content = Properties.Idioma.labelCorreoE;
            labelFotoPerfil.Content = Properties.Idioma.labelSeleccionarFotoPerfil; 

            labelCamposObligatorios.Inlines.Clear();
            labelCamposObligatorios.Inlines.Add(new Run("*") { Foreground = Brushes.Red });
            labelCamposObligatorios.Inlines.Add(new Run(Properties.Idioma.labelCamposObligatorios) { Foreground = Brushes.White });

            labelContraseniaInstruccion.Content = Properties.Idioma.labelContraseniaInstruccion;
            labelContraseniaMinimo.Content = Properties.Idioma.labelContraseniaMinimo;
            labelContraseniaMaximo.Content = Properties.Idioma.labelContraseniaMaximo;
            labelContraseniaSimbolos.Content = Properties.Idioma.labelContraseniaSimbolos;
            buttonCrearCuenta.Content = Properties.Idioma.buttonRegistrarse;
            buttonCambiarFoto.Content = Properties.Idioma.buttonCambiarFotoPerfil;
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
                //    string contraseniaHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(textBoxContrasenia.Password))).Replace("-", "");
                //    var servicioManejador = new ServicioManejador<ServicioRegistroClient>();
                //    bool resultado = servicioManejador.EjecutarServicio(proxy => { return proxy.RegistrarUsuario(/*AQUI LE PASO LAS CREDENCIA*/)});
                //    bool resultado = servicio.RegistrarUsuario(textBoxGamertag.Text, contraseniaHash);
                //    if (resultado == false)
                //    {
                //        //TODO: Manejar el error
                //        Console.WriteLine("error");
                //    }
            }
            catch (Exception)
            {
                //TODO: Manejar error
                MessageBox.Show("");
            }
            finally
            {
                if (servicio != null)
                {
                    ((ICommunicationObject)servicio).Close();
                }
            }
        }

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            contadorImagen = (contadorImagen + 1) % imagenesPerfil.Length;
            imgPerfil.Source = new BitmapImage(new Uri(imagenesPerfil[contadorImagen], UriKind.Relative));
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IniciarSesion iniciarSesionWindow = new IniciarSesion();
            iniciarSesionWindow.Show();
            this.Close();
        }
    }
}
