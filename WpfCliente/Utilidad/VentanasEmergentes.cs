using System;
using System.Windows;
using System.Windows.Controls;
using WpfCliente.Contexto;
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
            try
            {
                ventanaEmergente.Owner = ventana;
                ventanaEmergente.ShowDialog();
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }

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
            try
            {
                var ownerWindow = Window.GetWindow(userControl);
                if (ownerWindow != null)
                {
                    ventanaEmergente.Owner = ownerWindow;
                }
                ventanaEmergente.ShowDialog();
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }

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
            try
            {
                ventanaEmergente.Owner = ventana;
                ventanaEmergente.ShowDialog();
                SingletonGestorVentana.Instancia.RegresarSiNoEsInicio(ventana);
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }

        }

        public static string AbrirVentanaModalGamertag(Window ventana)
        {
            string valorObtenido = null;
            IngresarGamertagModalWindow ventanaModal = new IngresarGamertagModalWindow();
            try
            {
                ventanaModal.Owner = ventana;
                bool? resultado = ventanaModal.ShowDialog();

                if (resultado == true)
                {
                    valorObtenido = ventanaModal.ValorIngresado;
                }
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
                bool? resultado = ventanaModal.ShowDialog();

                if (resultado == true)
                {
                    valorObtenido = ventanaModal.ValorIngresado;
                }
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
                bool? resultado = ventanaModal.ShowDialog();
                if (resultado == true && !ventanaModal.ValorIngresado.Contains(" ") && ventanaModal.ValorIngresado != null)
                {
                    valorObtenido = ventanaModal.ValorIngresado;
                }
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
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
                bool? resultado = ventanaModal.ShowDialog();
                if (resultado == true && !ventanaModal.ValorIngresado.Contains(" "))
                {
                    valorObtenido = ventanaModal.ValorIngresado;
                }
            }
            catch (ArgumentException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }

            return valorObtenido;
        }
    }
}
