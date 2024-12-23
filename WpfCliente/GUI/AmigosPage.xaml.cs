﻿using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfCliente.Contexto;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class AmigosPage : Page, IHabilitadorBotones , IActualizacionUI
    {
        private int contadorClics = 0;
        private const int LIMITE_CLICS = 2;

        public AmigosPage()
        {
            KeepAlive = false;
            InitializeComponent();
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
        }

        private void ClicButtonFlechaAtras(object sender, MouseButtonEventArgs e)
        {
            SingletonGestorVentana.Instancia.Regresar();
        }
        private void ClicButtonFlechaRecargar(object sender, MouseButtonEventArgs e)
        {
            if (contadorClics >= LIMITE_CLICS)
            {
                imagenFlechaRecargar.Visibility = Visibility.Collapsed;
                return;
            }
            contadorClics++;
            RecargarSolicitudes();
        }

        private void RecargarSolicitudes()
        {
            try
            {
                var contenedorPadre = (Panel)listaSolicitudesAmistadUserControl.Parent;
                if (contenedorPadre != null)
                {
                    contenedorPadre.Children.Remove(listaSolicitudesAmistadUserControl);

                    ListaSolicitudesAmistadUserControl nuevaListaSolicitudes = new ListaSolicitudesAmistadUserControl();
                    contenedorPadre.Children.Add(nuevaListaSolicitudes);

                    listaSolicitudesAmistadUserControl = nuevaListaSolicitudes;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }

        private async void ClicButtonNuevaSolicitudAsync(object sender, MouseButtonEventArgs e)
        {
            await IntentarEnviarSolicitudAsync();
        }

        private async Task IntentarEnviarSolicitudAsync()
        {
            string gamertagSolicitud = ObtenerGamertagSolicitud();
            if (gamertagSolicitud == null)
            {
                return;
            }

            if (EsAutoSolicitud(gamertagSolicitud))
            {
                MostrarMensajeAutoSolicitud();
                return;
            }

            if (!await VerificarConexionAsync())
            {
                return;
            }

            if (!VerificarInicioSesion())
            {
                MostrarMensajeReiniciarSesion();
                return;
            }

            if (ValidacionesString.EsGamertagValido(gamertagSolicitud))
            {
                await ProcesarSolicitudAmistadAsync(gamertagSolicitud);
            }
            else
            {
                MostrarMensajeCamposInvalidos();
            }
        }

        private string ObtenerGamertagSolicitud()
        {
            return VentanasEmergentes.AbrirVentanaModalGamertag(Window.GetWindow(this));
        }

        private bool EsAutoSolicitud(string gamertagSolicitud)
        {
            return gamertagSolicitud.Equals(SingletonCliente.Instance.NombreUsuario);
        }

        private void MostrarMensajeAutoSolicitud()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloSolicitudAmistad,
                Properties.Idioma.mensajeNoAutoSolicitud,
                Window.GetWindow(this));
        }

        private async Task<bool> VerificarConexionAsync()
        {
            return await Conexion.VerificarConexionConBaseDatosSinCierreAsync(HabilitarBotones, Window.GetWindow(this));
        }

        private bool VerificarInicioSesion()
        {
            var servicio = new ServicioManejador<ServicioUsuarioClient>();
            return servicio.EjecutarServicio(llamada =>
                llamada.YaIniciadoSesion(SingletonCliente.Instance.NombreUsuario));
        }

        private void MostrarMensajeReiniciarSesion()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloErrorInesperado,
                Properties.Idioma.mensajeVulevaAIniciarSesion,
                Window.GetWindow(this));
        }

        private async Task ProcesarSolicitudAmistadAsync(string gamertagSolicitud)
        {
            try
            {
                if (await EnviarSolicitudAsync(gamertagSolicitud))
                {
                    MostrarMensajeSolicitudExitosa();
                }
            }
            catch (Exception excepcion)
            {
                ManejarExcepcion(excepcion);
            }
        }

        private void MostrarMensajeSolicitudExitosa()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloSolicitudAmistad,
                Properties.Idioma.mensajeSolicitudAmistadExitosa,
                Window.GetWindow(this));
        }

        private void ManejarExcepcion(Exception excepcion)
        {
            ManejadorExcepciones.ManejarExcepcionError(excepcion, Window.GetWindow(this));
        }

        private void MostrarMensajeCamposInvalidos()
        {
            VentanasEmergentes.CrearVentanaEmergente(
                Properties.Idioma.tituloSolicitudAmistad,
                Properties.Idioma.mensajeCamposInvalidos,
                Window.GetWindow(this));
        }


        private async Task<bool> EnviarSolicitudAsync(string gamertagReceptor)
        {
            bool conexionExitosa = await Conexion.VerificarConexionConBaseDatosSinCierreAsync(HabilitarBotones, Window.GetWindow(this));
            if (!conexionExitosa)
            {
                return false;
            }

            try
            {
                Usuario usuarioRemitente = new Usuario();
                usuarioRemitente.Nombre = SingletonCliente.Instance.NombreUsuario;
                usuarioRemitente.FotoUsuario = Imagen.ConvertirBitmapImageAMemoryStream(SingletonCliente.Instance.FotoJugador);

                var resultado = await SingletonCanal.Instancia.Amigos.EnviarSolicitudAmistadAsync(usuarioRemitente, gamertagReceptor);
                if (!resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloSolicitudAmistad, Properties.Idioma.mensajeSolicitudAmistadFallida, Window.GetWindow(this));
                }
                return resultado;
            }
            catch (FaultException<SolicitudAmistadFalla> excepcion)
            {
                if (excepcion.Detail.ExisteAmistad)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloSolicitudAmistad, Idioma.mensajeAmigoExistente, Window.GetWindow(this));
                }
                else if (excepcion.Detail.ExistePeticion)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloSolicitudAmistad, Idioma.mensajeSolicitudAmigoYaexistente, Window.GetWindow(this));
                }
                return false;
            }
            catch (FaultException excepcion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.mensajeErrorInesperado, excepcion.Message, Window.GetWindow(this));
                return false;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }
        }

        public void HabilitarBotones(bool esHabilitado)
        {
            imagenFlechaRecargar.IsEnabled = esHabilitado;
            imagenFlechaAtras.IsEnabled = esHabilitado;
            imagenAgregarAmigo.IsEnabled = esHabilitado;
            listaSolicitudesAmistadUserControl.IsEnabled = esHabilitado;
            imagenFlechaRecargar.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA: Utilidades.OPACIDAD_MINIMA;
            imagenFlechaAtras.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
            imagenAgregarAmigo.Opacity = esHabilitado ? Utilidades.OPACIDAD_MAXIMA : Utilidades.OPACIDAD_MINIMA;
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            this.Title = Properties.Idioma.tituloAmigos;
        }
    }
}
