using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {
        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window window)
        {
            if (window == null)
            {
                return;
            }
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow
            (
                tituloVentanaEmergente,
                descripcionVentanaEmergente);
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            var ownerWindow = Window.GetWindow(userControl);
            if (ownerWindow != null)
            {
                ventanaEmergente.Owner = ownerWindow;
            }

            ventanaEmergente.ShowDialog();
        }
        public static void CrearVentanaEmergenteConCierre(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window window)
        {
            if (window == null)
            {
                return;
            }
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
