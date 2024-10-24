﻿using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.Interfaz;
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
                ///TODO: Este debe ser el id de usuario (esto esta hecho para pruebas cambiar a una variable en despliegue)
                Usuario user = new Usuario
                {
                    IdUsuario = Singleton.Instance.IdUsuario,
                    Nombre = Singleton.Instance.NombreUsuario
                };
                // Ejecutamos las operaciones adicionales en tareas separadas
                var taskObtenerSesion = Task.Run(() => Conexion.UsuarioSesion.ObtenerSessionJugadorAsync(user));
                await Task.WhenAll(taskObtenerSesion);
                Thread.Sleep(10000);
                Conexion.Amigos.AbrirCanalParaPeticiones(user);

                // Esperamos a que ambas tareas se completen
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
            ventanaSala.Show();
            this.Hide();
            ventanaSala.Closed += (s, args) => {
                if (!Conexion.CerrarConexionesSalaConChat())
                {
                    MessageBox.Show("Error al tratar de conectarse con el servidor");
                    this.Close();   
                }
                this.Show(); 
            };
        }

        public void ObtenerSessionJugadorCallback(bool esSesionAbierta)
        {
            //TODO: No imitar el 418
            Console.WriteLine("Im a teapot");
        }

        private void ClicButtonUnirseSala(object sender, RoutedEventArgs e)
        {
            string codigoSala = AbrirVentanaModal();
            if (Validacion.ExisteSala(codigoSala))
            {
                AbrirVentanaSala(codigoSala);
                return;
            }
            else
            {
                //TODO: I18N
                MessageBox.Show("No existe la sala.");
            }


        }
        private string AbrirVentanaModal()
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            ventanaModal.Owner = this;
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }
            else
            {
                //TODO: I18N
                MessageBox.Show("No se ingresó ningún valor.");
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

        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //TODO: Pedirle a unaay los .resx
        }
    }
}
