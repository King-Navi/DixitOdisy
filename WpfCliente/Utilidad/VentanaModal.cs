using System;
using System.Windows;
using WpfCliente.GUI;
using WpfCliente.Interfaz;
using WpfCliente.Utilidad;

namespace WpfCliente.Utilidad
{
    public class VentanaModal 
    {
        public static string AbrirVentanaModalGamertag(Window window)
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = window;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static string AbrirVentanaModalSala(Window window)
        {
            string valorObtenido = null;
            UnirseSalaModalWindow ventanaModal = new UnirseSalaModalWindow();
            try
            {
                ventanaModal.Owner = window;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
            }
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static string AbrirVentanaModalCorreo(Window window)
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
            try
            {
                ventanaModal.Owner = window;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
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
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarComponentErrorException(ex);
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
