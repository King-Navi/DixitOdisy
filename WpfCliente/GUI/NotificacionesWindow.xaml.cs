using System;
using System.Collections.Generic;
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

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para NotificacionesWindow.xaml
    /// </summary>
    public partial class NotificacionesWindow : Window, IActualizacionUI
    {
        public NotificacionesWindow()
        {
            InitializeComponent();
        }

        public void ActualizarUI()
        {
            throw new NotImplementedException();
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
