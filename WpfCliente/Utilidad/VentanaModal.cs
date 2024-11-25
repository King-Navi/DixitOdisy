using System;
using System.Windows;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanaModal 
    {
        public static string AbrirVentanaModalGamertag(Window ventana)
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = ventana;
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
