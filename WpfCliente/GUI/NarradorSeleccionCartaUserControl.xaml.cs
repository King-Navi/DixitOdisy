using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class NarradorSeleccionCartaUserControl : UserControl
    {
        public ObservableCollection<ImagenCarta> Imagenes { get; set; }

        public NarradorSeleccionCartaUserControl(ObservableCollection<ImagenCarta> imagenes)
        {
            InitializeComponent();
            Imagenes = imagenes;
            this.Loaded += LoadedNarradorSeleccionCartaUsercontrol;
            DataContext = this;

        }


        private void LoadedNarradorSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window mainWindow)
            {
                mainWindow.MinHeight = this.MinHeight;
                mainWindow.MinWidth = this.MinWidth;
            }
        }

        private void UnloadedNarradorSeleccionCartaUsercontrol(object sender, RoutedEventArgs e)
        {
            this.Loaded -= LoadedNarradorSeleccionCartaUsercontrol;

        }
    }
}
