using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ResumenRondaUserControl : UserControl, INotifyPropertyChanged
    {
        //TODO: esto es solo si no queremos mostrar los nombres
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

        public ObservableCollection<Usuario> JugadorEstadisticas { get; set; }
        public ObservableCollection<string> Podio { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ResumenRondaUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ResumenRondaUserControl(ObservableCollection<Usuario> listaJugadores, ObservableCollection<string> podio)
        {
            InitializeComponent();
            DataContext = this;
            JugadorEstadisticas = listaJugadores;
            Podio = podio;
        }
        public void MostrarEnPodio(Usuario primerLugar, Usuario segundoLugar, Usuario tercerLugar)
        {
            Window window = Window.GetWindow(this);
            if (primerLugar != null)
            {
                Podio[0] = primerLugar?.Nombre ?? Properties.Idioma.labelNoHayPrimerLugar;
                PrimerLugar = Imagen.EsImagenValida(primerLugar.FotoUsuario, window) ? primerLugar : null; 
            }
            if (segundoLugar != null)
            {
                Podio[1] = segundoLugar?.Nombre ?? Properties.Idioma.labelNoHaySegundoLugar;
                SegundoLugar = Imagen.EsImagenValida(segundoLugar.FotoUsuario, window) ? segundoLugar : null; 
            }
            if (tercerLugar != null)
            {
                Podio[2] = tercerLugar?.Nombre ?? Properties.Idioma.labelNoHayTercerLugar;
                TercerLugar = Imagen.EsImagenValida(tercerLugar.FotoUsuario, window) ? tercerLugar : null; 
            }
        }
    }
}
