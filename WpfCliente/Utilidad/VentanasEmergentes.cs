using System.Windows;
using System.Windows.Controls;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {

        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente, Window window)
        {
            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente)
            {
                Owner = window
            };
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteErrorServidor(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorServidor;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorServidor;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;  
            ventanaEmergente.ShowDialog();
            CerrarSiNoEsInicioSesion(window);
        }

        public static void CrearVentanaEmergenteErrorBD(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorBaseDatos;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorBaseDatos;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.Show();
            CerrarSiNoEsInicioSesion(window);
        }


        public static void CrearVentanaEmergenteErrorInesperado(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorInesperado;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorInesperado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteLobbyNoEncontrado(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloLobbyNoEncontrado;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeLobbyNoEncontrado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteCodigoCopiado(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloCodigoCopiado;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeCodigoCopiado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteIdiomaInvalido(UserControl userControl)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloIdiomaInvalido;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeIdiomaInvalido;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = Window.GetWindow(userControl);
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteSesionIniciada(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloSesionIniciada; 
            string descripcionVentanaEmergente = Properties.Idioma.mensajeSesionIniciada;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteImagenInvalida(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloImagenInvalida;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeImagenInvalida;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteDatosEditadosExito(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloEditarUsuario;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeUsuarioEditadoConExito;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteCargarDatosAmigosFalla(UserControl userControl)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloCargarAmigosFalla;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeCargarAmigosFalla;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = Window.GetWindow(userControl);
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteCorreoYGamertagNoCoinciden(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloCorreoYGamertagNoCoinciden;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeCorreoYGamertagNoCoinciden;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteInvitacionEnviada(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloInvitacionPartida;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeInvitacionExitosa;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteInvitacionNoEnviada(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloInvitacionPartida;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeInvitacionFallida;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteSolicitudEnviada(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloSolicitudAmistad;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeSolicitudAmistadExitosa;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteSolicitudAceptada(Window window, string gamertag)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloSolicitudAmistad;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeSolicitudAmistadAceptada + gamertag;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
            window.Close();
        }

        public static void CrearVentanaEmergenteSolicitudRechazada(Window window, string gamertag)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloSolicitudAmistad;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeSolicitudAmistadRechazada + gamertag;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;
            ventanaEmergente.ShowDialog();
            window.Close();
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
