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

        public static string AbrirVentanaModalGamertag(Window ventana)
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = ventana;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static string AbrirVentanaModalSala(Window ventana)
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            try
            {
                ventanaModal.Owner = ventana;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static string AbrirVentanaModalCorreo(Window ventana)
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
            try
            {
                ventanaModal.Owner = ventana;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            bool? resultado = ventanaModal.ShowDialog();
            if (resultado == true && !ventanaModal.ValorIngresado.Contains(" ") && ventanaModal.ValorIngresado != null)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }


            return valorObtenido;
        }

        public static string AbrirVentanaModalCorreo(Window window, bool olvidoContrasenia)
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow(olvidoContrasenia);
            try
            {
                ventanaModal.Owner = window;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
            }
            bool? resultado = ventanaModal.ShowDialog();
            if (resultado == true && !ventanaModal.ValorIngresado.Contains(" "))
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }
    }
}
