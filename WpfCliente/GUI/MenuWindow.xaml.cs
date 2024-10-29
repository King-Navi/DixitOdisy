﻿using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window, IServicioUsuarioSesionCallback, IActualizacionUI
    {
        public MenuWindow()
        {
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();
            AbrirConexiones();
        }

        private async void AbrirConexiones()
        {
            try
            {
                var resultadoUsuarioSesion = await Conexion.AbrirConexionUsuarioSesionCallbackAsync(this);
                var resultadoAmigo = await Conexion.AbrirConexionAmigosCallbackAsync(amigosUserControl);
                if (!resultadoAmigo || !resultadoUsuarioSesion)
                {
                    MessageBox.Show("Error al conctase al servidor");
                    this.Close();
                }
                Usuario user = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };
                //TODO: Hay que verificar que no haya iniciado sesion antes
                Conexion.UsuarioSesion.ObtenerSessionJugador(user);
            }
            catch (Exception excepcion)
            {
                this.Close();
            };
        }

        private void ClicBotonCrearSala(object sender, RoutedEventArgs e)
        {
            //TODO: Hacer la logica para la peticion al servidor de la sala y la respuesta, este es el caso en el que el solicitante es el anfitrion
            AbrirVentanaSala(null);
        }
        private void AbrirVentanaSala(string idSala)
        {
            SalaEspera ventanaSala = new SalaEspera(idSala);
            try
            {
                ventanaSala.Show();

            }
            catch (InvalidOperationException)
            {
                VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                this.Close();
                return;
            }
            this.Hide();
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
                    this.Close();   
                }
                this.Show(); 
            };
        }

        public void ObtenerSessionJugadorCallback(bool esSesionAbierta)
        {
            Usuario user = new Usuario
            {
                IdUsuario = Singleton.Instance.IdUsuario,
                Nombre = Singleton.Instance.NombreUsuario
            };
            Conexion.Amigos.AbrirCanalParaPeticiones(user);

            //TODO: No imitar el 418
            Console.WriteLine("Im a teapot");
        }

        private async void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            string codigoSala = AbrirVentanaModal();
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (codigoSala != null)
            {
                if (Validacion.ExisteSala(codigoSala))
                {
                    AbrirVentanaSala(codigoSala);
                    return;
                }
                else
                {
                    //TODO: I18N
                    VentanasEmergentes.CrearVentanaEmergenteLobbyNoEncontrado(this);
                }

            }


        }

        private void HabilitarBotones(bool v)
        {
            //TODO
        }

        private string AbrirVentanaModal()
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            try
            {
                ventanaModal.Owner = this;

            }
            catch (Exception)
            {
                
            }            
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }
            

            return valorObtenido;
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                Conexion.CerrarUsuarioSesion();
                Conexion.CerrarConexionesSalaConChat();
            }
            catch (Exception excepcion)
            {
                //TODO Manejar el error
            }
            IniciarSesion iniciarSesion = new IniciarSesion();
            iniciarSesion.Show();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            buttonCrearSala.Content = Idioma.buttonCrearSalaEspera;
            buttonUniserSala.Content = Idioma.buttonUnirseSalaDeEspera;
            buttonSalir.Content = Idioma.buttonCerrarSesion;
            //TODO: Pedirle a unaay los .resx
        }

        private void buttonSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
