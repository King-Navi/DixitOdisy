using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class UsuarioUserControl : UserControl
    {
        public UsuarioUserControl()
        {
            InitializeComponent();
            FondoColorAleatorio();
        }

        private void FondoColorAleatorio()
        {
            this.Background = Utilidades.ObtenerColorAleatorio();
        }
    }
}