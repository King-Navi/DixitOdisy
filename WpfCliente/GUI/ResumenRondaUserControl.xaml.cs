using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ResumenRondaUserControl : UserControl, INotifyPropertyChanged
    {
        private Usuario _primerLugar;
        public Usuario PrimerLugar
        {
            get => _primerLugar;
            set
            {
                _primerLugar = value;
                OnPropertyChanged();
            }
        }

        private Usuario _segundoLugar;
        public Usuario SegundoLugar
        {
            get => _segundoLugar;
            set
            {
                _segundoLugar = value;
                OnPropertyChanged();
            }
        }

        private Usuario _tercerLugar;
        public Usuario TercerLugar
        {
            get => _tercerLugar;
            set
            {
                _tercerLugar = value;
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
            InitializeComponent();
            DataContext = this;
            Jugadores = new ObservableCollection<JugadorTablaPuntaje>
            {
                new JugadorTablaPuntaje { Nombre = "Jugador 1", Puntos = 100, MostrarBoton = true },
                new JugadorTablaPuntaje { Nombre = "Jugador 2", Puntos = 50, MostrarBoton = false },
                new JugadorTablaPuntaje { Nombre = "Jugador 3", Puntos = 75, MostrarBoton = true }
            };
        }

        private void MostrarPodio()
        {
            PrimerLugar = SingletonPartida.Instancia.PrimerLugar;
            SegundoLugar = SingletonPartida.Instancia.SegundoLugar;
            TercerLugar = SingletonPartida.Instancia.TercerLugar;
        }

        private void ClicButtonExpulsar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext  is JugadorTablaPuntaje jugador)
            {
                MessageBox.Show($"¡Clic en el botón de {jugador.Nombre} con puntaje {jugador.Puntos}!");
            }

        }
    }
}
