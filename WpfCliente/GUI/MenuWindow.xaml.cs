using System;
using System.Collections.Generic;
using System.Linq;
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
using UtilidadesLibreria;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback , IActualizacionUI
    {
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            Singleton.Instance.ServicioUsuarioSesionCliente = new ServidorDescribelo.ServicioUsuarioSesionClient(new InstanceContext(this));
            ServidorDescribelo.IServicioUsuarioSesion sesion = Singleton.Instance.ServicioUsuarioSesionCliente;
            ///Este debe ser el id de usuario (esto esta hecho para pruebas cambiar a una variable en despliegue)
            sesion.ObtenerSessionJugador(new ServidorDescribelo.Usuario { IdUsuario = 1, });
        }

        private void ClicBotonCrearSala(object sender, RoutedEventArgs e)
        {
            //TODO: Hacer la logica para la peticion al servidor de la sala y la respuesta, este es el caso en el que el solicitante es el anfitrion
            AbrirVentanaSala(null);
        }
        private void AbrirVentanaSala(string idSala)
        {
            SalaEspera ventanaSala = new SalaEspera(idSala);
            this.Hide();
            ventanaSala.Show();
            ventanaSala.Closed += (s, args) => this.Show();
        }

        public void ObtenerSessionJugadorCallback(bool esSesionAbierta)
        {
            Console.WriteLine("Im a teapot");
        }

        private void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            string codigoSala= AbrirVentanaModal();
            if (Validacion.ExisteSala(codigoSala))
            {
                AbrirVentanaSala(codigoSala);
                return;
            }
            //TODO: I18N
            MessageBox.Show("No existe la sala.");
            
        }

        

        private string AbrirVentanaModal()
        {
            UniserSalaModalWindow inputWindow = new UniserSalaModalWindow();
            inputWindow.Owner = this; // Establecer la ventana propietaria
            bool? resultado = inputWindow.ShowDialog();

            if (resultado == true)
            {
                string valorObtenido = inputWindow.ValorIngresado;
                return valorObtenido;
            }
            else
            {
                //TODO: I18N
                MessageBox.Show("No se ingresó ningún valor.");
            }
            return null;
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            if (Singleton.Instance.ServicioUsuarioSesionCliente != null)
            {
                try
                {
                    Singleton.Instance.ServicioUsuarioSesionCliente.Close();
                }
                catch (Exception ex)
                {
                    //TODO Manejar el error
                }
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            throw new NotImplementedException();
        }
    }
}
