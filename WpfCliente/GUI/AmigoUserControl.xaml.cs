﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for AmigoUserControl.xaml
    /// </summary>
    public partial class AmigoUserControl : UserControl
    {
        public AmigoUserControl()
        {
            InitializeComponent();
            SetFondoColorAleatorio();
        }
        public AmigoUserControl( Amigo amigo)
        {
            InitializeComponent();
            labelNombreAmigo.Content = amigo.Nombre;
            labelEstadoAmigo.Content = amigo.Estado;
            imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(amigo.Foto);
            SetFondoColorAleatorio();
            
        }

        private void SetFondoColorAleatorio()
        {
            this.Background = Utilidades.GetColorAleatorio();
        }
    }
}
