using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {
        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window ventana)
        {
            if (ventana == null)
            {
                return;
            }
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow
            (
                tituloVentanaEmergente,
                descripcionVentanaEmergente);
            ventanaEmergente.Owner = ventana;
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
        public static void CrearVentanaEmergenteConCierre(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window ventana)
        {
            if (ventana == null)
            {
                return;
            }
            VentanaEmergenteModalWindow ventanaEmergente = new VentanaEmergenteModalWindow(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = ventana;  
            ventanaEmergente.ShowDialog();
            CerrarSiNoEsInicioSesion(ventana);
        }

        private static void CerrarSiNoEsInicioSesion(Window ventana)
        {
            if (!(ventana is IniciarSesion))
            {
                ventana.Close();
            }
        }
    }
}
