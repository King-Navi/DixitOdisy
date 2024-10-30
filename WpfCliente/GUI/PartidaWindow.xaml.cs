using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    

    public partial class PartidaWindow : Window , IActualizacionUI , IHabilitadorBotones, IServicioPartidaSesionCallback, INotifyPropertyChanged
    {
        private class ImagenesCompartidas
        {
            public ObservableCollection<ImagenCarta> Imagenes { get; } = new ObservableCollection<ImagenCarta>();
        }
        public ICommand ComandoImagenGlobal { get; set; } //testing


        private ImagenesCompartidas imagenesCompartidas;
        public event PropertyChangedEventHandler PropertyChanged;
        private int pantallaActual = 1;
        SeleccionCartaUsercontrol seleccionCartasUserControl;
        NarradorSeleccionCartaUserControl narradorSeleccionCartasUserControl;
        private bool esNarrador;
        public bool EsNarrador
        {
            get => esNarrador;
            set
            {
                esNarrador = value;
                OnPropertyChanged();
            }
        }
        public int PantallaActual
        {
            get => pantallaActual;
            set
            {
                pantallaActual = value;
                OnPropertyChanged();
            }
        }
        public PartidaWindow(string idPartida)
        {
            InitializeComponent();
            ActualizarUI();
            UnirsePartida(idPartida);
            EsNarrador = true;
            DataContext = this;
            InicializarComponenetes();
        }

        private void InicializarComponenetes()
        {
            imagenesCompartidas = new ImagenesCompartidas();
            ComandoImagenGlobal = new ComandoRele<string>(ComandoImagenPorId);



            narradorSeleccionCartasUserControl = new NarradorSeleccionCartaUserControl(imagenesCompartidas.Imagenes);

            seleccionCartasUserControl = new SeleccionCartaUsercontrol(imagenesCompartidas.Imagenes);

            gridPantalla1.Children.Add(seleccionCartasUserControl);
            gridPantalla2.Children.Add(narradorSeleccionCartasUserControl);
            // Lógica para remover `gridPantalla1` del `Grid` principal
            //if (this.Content is Grid mainGrid)
            //{
            //    mainGrid.Children.Remove(gridPantalla2);
            //}
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AvanzarPantalla() //Fixme o algo por el estilo
        {
            //if (EsNarrador)
            //{
            //    PantallaActual = 1; // El narrador siempre ve la pantalla X
            //}
            //else
            //{
            //    PantallaActual = ; // Ciclo para jugadores
            //}
            PantallaActual++;
            if (PantallaActual == 3)
            {
                pantallaActual = 1;
            }
            //ActualizarDataContext();
        }

        private void ActualizarDataContext()
        {
            if (PantallaActual == 1)
            {
                gridPantalla1.DataContext = seleccionCartasUserControl;
            }
            else if (PantallaActual == 2)
            {
                gridPantalla2.DataContext = narradorSeleccionCartasUserControl;
            }
            // Y así sucesivamente para otras pantallas
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

        private async void ClicButtonConfirmarTurno(object sender, RoutedEventArgs e) //FIXME esat en pruebas
        {
            AvanzarPantalla();
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
            imagenesCompartidas.Imagenes.Add(imagen);

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

        private void CerrandoVentana(object sender, CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                Conexion.CerrarChatMotor();
                Conexion.CerrarConexionesPartida();
            }
            catch (Exception excepcion)
            {
                //TODO Manejar excepcion
            }
        }
        public void ComandoImagenPorId(string id)
        {
            if (EsNarrador)
            {
                EscogerImagenNarrador(id);
            }
            else
            {
                EliminarImagenPorId(id);
            }
        }
        public void EliminarImagenPorId(string id)
        {
            var imagenAEliminar = imagenesCompartidas.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEliminar != null)
            {
                imagenesCompartidas.Imagenes.Remove(imagenAEliminar);
            }
        }
        public void EscogerImagenNarrador(string id)
        {
            var imagenAEscoger = imagenesCompartidas.Imagenes.FirstOrDefault(i => i.IdImagen == id);
            if (imagenAEscoger == null)
                return;
            MostrarCartaModelWindow ventanaModal = new MostrarCartaModelWindow(true, imagenAEscoger.BitmapImagen);
            bool? resultado = ventanaModal.ShowDialog();
            var pista = ventanaModal.Pista;
            if ((bool)resultado)
            {
                //TODO: El narrador escogio esta ID de imagen y ya dio el promp (pista)
                Console.WriteLine($"El narrador ya escogio {pista}");
                imagenesCompartidas.Imagenes.Remove(imagenAEscoger);
            }
        }
    }
}
