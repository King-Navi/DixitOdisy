using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for PartidaWindow.xaml
    /// </summary>
    public partial class PartidaWindow : Window , IActualizacionUI , IHabilitadorBotones, IServicioPartidaSesionCallback
    {
        
        public PartidaWindow(string idPartida)
        {
            InitializeComponent();
            ActualizarUI();
            UnirsePartida(idPartida);
            DataContext = this;
        }
        private async void UnirsePartida(string idPartida)
        {
            bool conexionExitosa = await Conexion.VerificarConexion(HabilitarBotones, this);
            if (!conexionExitosa)
            {
                return;
            }
            if (!Validacion.ExistePartida(idPartida))
            {
                //No existe la partida ¿¿??
                NoHayConexion();
                return;
            }
            var resultadoTask = Conexion.AbrirConexionPartidaCallbackAsync(this);
            bool resultado = resultadoTask.Result;

            if (!resultado)
            {
                NoHayConexion();
                return;
            }
            Conexion.Partida.UnirsePartida(Singleton.Instance.NombreUsuario, idPartida);
            //TODO: UnirseChat();

        }
       
        public void ActualizarUI()
        {
            //TODO: recursos .resx
        }

        public void AvanzarRondaCallback()
        {
            throw new NotImplementedException();
        }

        public void HabilitarBotones(bool esValido)
        {
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        private async void ClicButtonConfirmarTurno(object sender, RoutedEventArgs e)
        {
            //TODO: Verficar que seleciono imagen
            try
            {
                await Conexion.Partida.SolicitarImagenCartaAsync(Singleton.Instance.NombreUsuario, Singleton.Instance.IdPartida);
            }
            catch (Exception ex)
            { 
            }
        }
        private void NoHayConexion()
        {
            this.Close();
        }

        public void AvanzarRondaCallback(int RondaActual)
        {
            throw new NotImplementedException();
        }

        public void TurnoPerdidoCallback()
        {
            throw new NotImplementedException();
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            IMAGEN1.Source = Imagen.ConvertirStreamABitmapImagen(imagen.ImagenStream);
            if (imagen != null)
            {
                mazoUserControl.RecibirImagen(imagen);
            };
        }

        public void FinalizarPartida()
        {
            throw new NotImplementedException();
        }

        public void ObtenerJugadorPartidaCallback(Usuario jugardoreNuevoEnSala)
        {
        }

        public void EliminarJugadorPartidaCallback(Usuario jugardoreRetiradoDeSala)
        {
            throw new NotImplementedException();
        }
    }
}
