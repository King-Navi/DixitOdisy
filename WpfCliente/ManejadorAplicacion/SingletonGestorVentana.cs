using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WpfCliente.GUI;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.Utilidad;
using System.Linq;


namespace WpfCliente.Contexto
{
    public class SingletonGestorVentana
    {
        private Window VentanaPrincipal { get; set; }
        private static readonly Lazy<SingletonGestorVentana> instancia =
            new Lazy<SingletonGestorVentana>(() => new SingletonGestorVentana());
        public static SingletonGestorVentana Instancia => instancia.Value;
        private SingletonGestorVentana() { }
        public void Iniciar()
        {
            
            PrincipalWindow ventanaPrincipal = new PrincipalWindow();
            VentanaPrincipal = ventanaPrincipal;
            ventanaPrincipal.Show();
        }

        public bool AbrirNuevaVentanaPrincipal(Window ventanaNueva)
        {
            try
            {
                ventanaNueva.Show();
                VentanaPrincipal.Close();
                VentanaPrincipal = ventanaNueva;
                return true;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

            public bool LimpiarHistorial()
        {
            try
            {
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco != null)
                {
                    while (marco.CanGoBack)
                    {
                        marco.RemoveBackEntry();
                    }
                    return true;
                }

            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

        public bool NavegarA(Page nuevoMarco)
        {
            try
            {
                EvaluarMarcoNulo(nuevoMarco);
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
               
                if (marco != null)
                {
                    marco.Navigated -= MarcoNavigacion;
                    EvaluarSiEsInicio(nuevoMarco);
                    marco.Navigate(nuevoMarco);
                    return true;
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

        private void EvaluarSiEsInicio(Page nuevaPagina)
        {
            try
            {
                EvaluarMarcoNulo(nuevaPagina);
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco == null)
                {
                    throw new ArgumentNullException(nameof(nuevaPagina));
                }
                if (nuevaPagina is IniciarSesionPage)
                {
                    CerrarTodaConexion();
                    LimpiarHistorial();
                    return;
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }
        private void EvaluarSiEsMenu(Page nuevaPagina)
        {
            try
            {
                EvaluarMarcoNulo(nuevaPagina);
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco == null)
                {
                    throw new ArgumentNullException(nameof(nuevaPagina));
                }
                if (nuevaPagina is MenuPage)
                {
                    CerrarConexionSalaPartida();
                    LimpiarHistorial();
                    return;
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        public void Regresar()
        {
            try
            {
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco != null && marco.CanGoBack)
                {
                    marco.Navigated += MarcoNavigacion;
                    marco.GoBack();
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        private void MarcoNavigacion(object sender, NavigationEventArgs e)
        {
            if (!(sender is Frame marco)) 
            { 
                return; 
            }
            if (marco != null)
            {
                marco.ContentRendered += MarcoLectorContenido;
                marco.Navigated -= MarcoNavigacion;
            }
        }
        private void MarcoLectorContenido(object sender, EventArgs e)
        {
            if (!(sender is Frame marcoViejo))
            {
                return;
            }
            marcoViejo.ContentRendered -= MarcoLectorContenido;
            try
            {
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco?.Content is Page paginaActual)
                {
                    EvaluarSiEsInicio(paginaActual);
                    EvaluarSiEsMenu(paginaActual);
                    EvaluarSiEsSala(paginaActual);
                    if (marcoViejo != null)
                    {
                        marcoViejo.ContentRendered -= MarcoLectorContenido;
                    }
                    return;
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            finally
            {
                marcoViejo.ContentRendered -= MarcoLectorContenido;
            }
        }

        private void EvaluarSiEsSala(Page paginaActual)
        {
            try
            {
                EvaluarMarcoNulo(paginaActual);
                var ventana = VentanaPrincipal as INavegacion;
                var marco = ventana?.MarcoNavegacion;
                if (marco == null)
                {
                    throw new ArgumentNullException(nameof(paginaActual));
                }
                if (paginaActual is SalaEsperaPage)
                {
                    CerrarConexionSala();
                    LimpiarHistorial();
                    return;
                }
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            };
        }

        private void EvaluarMarcoNulo(Page pagina)
        {
            if (pagina == null)
            {
                throw new ArgumentNullException();
            }
        }

        public void RegresarSiNoEsInicio(Window ventana)
        {
            try
            {
                var ventanaConInterfazValida = ventana as INavegacion;
                if (ventanaConInterfazValida?.MarcoNavegacion == null)
                {
                    return;
                }
                var paginaActual = ventanaConInterfazValida.MarcoNavegacion.Content as Page;
                if (paginaActual == null)
                {
                    return;
                }
                if (!(paginaActual is IniciarSesionPage))
                {
                    if (ventanaConInterfazValida.MarcoNavegacion.CanGoBack)
                    {
                        ventanaConInterfazValida.MarcoNavegacion.GoBack();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
        }


        private void CerrarTodaConexion()
        {
            SingletonCanal.Instancia.CerrarTodaConexion();
            Conexion.CerrarPartida();
            SingletonChat.Instancia.CerrarConexionChat();
            SingletonSalaJugador.Instancia.CerrarConexion();
        }
        private void CerrarConexionSalaPartida()
        {
            Conexion.CerrarPartida();
            SingletonChat.Instancia.CerrarConexionChat();
            SingletonSalaJugador.Instancia.CerrarConexion();
        } 
        private void CerrarConexionSala()
        {
            SingletonChat.Instancia.CerrarConexionChat();
            SingletonSalaJugador.Instancia.CerrarConexion();
        }
    }

    
}


