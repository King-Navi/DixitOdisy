using System.Windows;
using System.Windows.Controls;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {
        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window window)
        {
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow
            (
                tituloVentanaEmergente,
                descripcionVentanaEmergente);
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, UserControl userControl)
        {
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = Window.GetWindow(userControl);
            ventanaEmergente.ShowDialog();
        }
        public static void CrearVentanaEmergenteConCierre(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window window)
        {
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;  
            ventanaEmergente.ShowDialog();
            CerrarSiNoEsInicioSesion(window);
        }

        private static void CerrarSiNoEsInicioSesion(Window window)
        {
            if (!(window is IniciarSesion))
            {
                window.Close();
            }
        }
    }
}
