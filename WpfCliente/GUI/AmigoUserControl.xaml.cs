﻿using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class AmigoUserControl : UserControl, IHabilitadorBotones
    {
        public AmigoUserControl()
        {
            InitializeComponent();
            ColocarFondoColorAleatorio();
        }
        public AmigoUserControl( Amigo amigo)
        {
            InitializeComponent();
            labelNombreAmigo.Content = amigo.Nombre;
            labelEstadoAmigo.Content = amigo.Estado;
            labelUltimaConexion.Content = amigo.UltimaConexion;
            imageAmigo.Source = Imagen.ConvertirStreamABitmapImagen(amigo.Foto);
            ColocarFondoColorAleatorio();
            
        }

        private void ColocarFondoColorAleatorio()
        {
            this.Background = Utilidades.ObtenerColorAleatorio();
        }

        private void ClicButtonEliminarAmigo(object sender, RoutedEventArgs e)
        {
            _ = EliminarAmigoAsync();
        }

        private async Task<bool> EliminarAmigoAsync()
        {
            bool conexionExitosa = await Conexion.VerificarConexionAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return false;
            }
            try
            {
                return await SingletonCanal.Instancia.Amigos.EliminarAmigoAsync(SingletonCliente.Instance.NombreUsuario, labelNombreAmigo.Content.ToString());
            }
            catch (FaultException<ServidorFalla> excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            menuItemEliminarAmigo.IsEnabled = esHabilitado;
        }
    }
}
