using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ResumenRondaUserControl : UserControl, INotifyPropertyChanged, IActualizacionUI
    {
        private Usuario primerLugar;
        public Usuario PrimerLugar
        {
            get => primerLugar;
            set
            {
                primerLugar = value;
                OnPropertyChanged();
            }
        }

        private Usuario segundoLugar;
        public Usuario SegundoLugar
        {
            get => segundoLugar;
            set
            {
                segundoLugar = value;
                OnPropertyChanged();
            }
        }

        private Usuario tercerLugar;
        public Usuario TercerLugar
        {
            get => tercerLugar;
            set
            {
                tercerLugar = value;
                OnPropertyChanged();
            }
        }

        private string expulsarPropiedad;
        public string ExpulsarPropiedad
        {
            get => expulsarPropiedad;
            set
            {
                expulsarPropiedad = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<JugadorTablaPuntaje> Jugadores { get; set; } = new ObservableCollection<JugadorTablaPuntaje>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ResumenRondaUserControl()
        {
            SingletonPartida.Instancia.EstadisticasEnviadas += MostrarPodio;
            SingletonPartida.Instancia.SeActualizoPuntajes += SeActualizoPuntaje;
            InitializeComponent();
            DataContext = this;
            Jugadores = new ObservableCollection<JugadorTablaPuntaje>
            {
                new JugadorTablaPuntaje { Nombre = "Jugador 1", Puntos = 100, MostrarBoton = true },
                new JugadorTablaPuntaje { Nombre = "Jugador 2", Puntos = 50, MostrarBoton = false },
                new JugadorTablaPuntaje { Nombre = "Jugador 3", Puntos = 75, MostrarBoton = true }
            };
            ActualizarUI();
        }

        private void SeActualizoPuntaje(List<JugadorTablaPuntaje> lista)
        {
            if (lista == null)
            {
                return;
            }
            try
            {
                Jugadores = new ObservableCollection<JugadorTablaPuntaje>(lista);
                OnPropertyChanged(nameof(Jugadores));
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        private void MostrarPodio()
        {
            PrimerLugar = SingletonPartida.Instancia.PrimerLugar;
            SegundoLugar = SingletonPartida.Instancia.SegundoLugar;
            TercerLugar = SingletonPartida.Instancia.TercerLugar;
        }

        private async void ClicButtonExpulsarAsync(object sender, RoutedEventArgs e)
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(null, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                Application.Current.Shutdown();
                return;
            }
            if (sender is Button boton && boton.DataContext is JugadorTablaPuntaje jugador)
            {
                boton.IsEnabled = false;
                boton.Visibility = Visibility.Hidden;

                try
                {
                    if (SingletonCliente.Instance.NombreUsuario.Equals(jugador.Nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException();
                    }
                    var resultado =await SingletonPartida.Instancia.Partida.ExpulsarJugadorPartidaAsync(
                        SingletonCliente.Instance.NombreUsuario,
                        jugador.Nombre,
                        SingletonCliente.Instance.IdPartida);
                    if (resultado)
                    {
                        boton.IsEnabled = false;
                        boton.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        boton.IsEnabled = true;
                        boton.Visibility = Visibility.Visible;
                    }
                }
                catch (ArgumentException)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloExpulsionInvalida, Idioma.mensajeNoAutoExpulsion, Window.GetWindow(this));
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                }
            }

        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            ExpulsarPropiedad = Idioma.buttonExpulsar;
        }
    }
}
