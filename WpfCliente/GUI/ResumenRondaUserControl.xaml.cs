using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ResumenRondaUserControl.xaml
    /// </summary>
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
            if (primerLugar != null)
            {
                Podio[0] = primerLugar?.Nombre ?? "No hay primer lugar";
                PrimerLugar = Imagen.EsImagenValida(primerLugar.FotoUsuario) ? primerLugar : null; 
            }
            if (segundoLugar != null)
            {
                Podio[1] = segundoLugar?.Nombre ?? "No hay segundo lugar";
                SegundoLugar = Imagen.EsImagenValida(segundoLugar.FotoUsuario) ? segundoLugar : null; 
            }
            if (tercerLugar != null)
            {
                Podio[2] = tercerLugar?.Nombre ?? "No hay tercer lugar";
                TercerLugar = Imagen.EsImagenValida(tercerLugar.FotoUsuario) ? tercerLugar : null; 
            }
        }
    }
}
