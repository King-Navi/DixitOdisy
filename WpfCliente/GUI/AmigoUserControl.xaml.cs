using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
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
        private void ColocarFondoColorAleatorio()
        {
            this.Background = Utilidades.ObtenerColorAleatorio();
        }

        private async void ClicButtonEliminarAmigoAsync(object sender, RoutedEventArgs e)
        {
            await EliminarAmigoAsync();
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
            catch (TimeoutException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            return false;
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            menuItemEliminarAmigo.IsEnabled = esHabilitado;
        }
    }
}
