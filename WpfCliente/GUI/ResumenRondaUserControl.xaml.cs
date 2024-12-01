using System;
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
        }

        private void MostrarPodio()
        {
            PrimerLugar = SingletonPartida.Instancia.PrimerLugar;
            SegundoLugar = SingletonPartida.Instancia.SegundoLugar;
            TercerLugar = SingletonPartida.Instancia.TercerLugar;
        }
    }
}
