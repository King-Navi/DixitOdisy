using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class EditarPerfilWindow : Window , IActualizacionUI
    {
        private bool cambioImagen = false;
        public EditarPerfilWindow()
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            CargarDatos();
            ActualizarUI();
        }

        private void CargarDatos()
        {
            textBoxCorreo.Text = Singleton.Instance.Correo;
            CargarImagen();
        }

        private void ClicButtonCambiarImagen(object sender, RoutedEventArgs e)
        {
            string rutaImagen = AbrirDialogoSeleccionImagen();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (EsImagenValida(rutaImagen))
                {
                    ProcesarImagen(rutaImagen);
                }
                else
                {
                    MostrarError("El archivo seleccionado no es una imagen válida. Por favor selecciona una imagen.");
                }
            }
            else
            {
                MostrarError("No se seleccionó ninguna imagen.");
            }
        }
        private string AbrirDialogoSeleccionImagen()
        {
            return Imagen.SelecionarRutaImagen();
        }

        private bool EsImagenValida(string rutaImagen)
        {
            return Imagen.EsImagenValida(rutaImagen);
        }
        private void ProcesarImagen(string rutaImagen)
        {
            VentanasEmergentes.CrearVentanaEmergente("Imagen seleccionada", rutaImagen, this);
            // Aquí puedes asignar la imagen a un control de imagen o hacer más procesamiento
            cambioImagen = true;
        }
        private void MostrarError(string mensaje)
        {
            VentanasEmergentes.CrearVentanaEmergenteErrorInesperado(this);
        }

        private void CargarImagen()
        {
            try
            {
                BitmapImage bitmap = Singleton.Instance.FotoJugador;
                imageFotoJugador.Source = bitmap;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelNombreJugador.Content = Singleton.Instance.NombreUsuario;
            buttonAceptarCambio.Content = Properties.Idioma.buttonAceptar;
            buttonCancelarCambio.Content = Properties.Idioma.buttonCancelar;
            buttonCambiarFoto.Content = Properties.Idioma.buttonCambiarFotoPerfil;
        }

        private void clicButtonCancelar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clicButtonAceptar(object sender, RoutedEventArgs e)
        {
            bool realizoCambios = false;
            Usuario usuarioEditado = new Usuario();

            // Verificar si hay un cambio en la imagen
            if (cambioImagen && imageFotoJugador.Source is BitmapImage bitmapImage)
            {
                usuarioEditado.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(bitmapImage);
                realizoCambios = true;
            }

            // Verificar cambio en el correo
            if (!string.IsNullOrWhiteSpace(textBoxCorreo.Text)
                && !textBoxCorreo.Text.Contains(" ")
                && textBoxCorreo.Text != Singleton.Instance.Correo)
            {
                usuarioEditado.Correo = textBoxCorreo.Text;
                realizoCambios = true;
            }

            // Verificar cambio en la contraseña
            if (!string.IsNullOrEmpty(textBoxContrania.Text)
                && textBoxContrania.Text == textBoxConfirmacionContrania.Text
                && textBoxContrania.Text != Singleton.Instance.ContraniaHash)
            {
                usuarioEditado.ContraseniaHASH = Encriptacion.OcuparSHA256(textBoxContrania.Text);
                realizoCambios = true;
            }
            // Si hubo cambios, asignar otros valores críticos y llamar al servicio
            if (realizoCambios)
            {
                usuarioEditado.IdUsuario = Singleton.Instance.IdUsuario;
                usuarioEditado.Nombre = Singleton.Instance.NombreUsuario;

                var manejadorServicio = new ServicioManejador<ServicioUsuarioClient>();
                bool  resultado = manejadorServicio.EjecutarServicio(proxy =>
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
                    MessageBox.Show("No se editaron tus datos");

                }
            }
            else 
            {
                MessageBox.Show("No hiciste nada");
            }
    
        }
    }
}
