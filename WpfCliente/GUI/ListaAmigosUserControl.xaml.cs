using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ListaAmigosUserControl : UserControl, IServicioAmistadCallback , IActualizacionUI
    {
        public ObservableCollection<Amigo> Amigos { get; set; } = new ObservableCollection<Amigo>();
        private bool desechado = false;
        private DispatcherTimer timer;
        private DateTime ultimaActualizacion;
        private const string FORMATO_HORA = "HH:mm:ss";
        public ListaAmigosUserControl()
        {
            InitializeComponent();
            IniciarHora();
            DataContext = this;
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            ActualizarUI();

        }

        private void IniciarHora()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += HoraActual;
            timer.Start();
        }

        [DebuggerStepThrough]
        private void HoraActual(object sender, EventArgs e)
        {
            DateTime horaActual = DateTime.Now;
            if (horaActual.Second != ultimaActualizacion.Second)
            {
                labelHora.Content = horaActual.ToString(FORMATO_HORA);
                ultimaActualizacion = horaActual;
            }
        }

        private void Desechar()
        {
            if (desechado) return;
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= HoraActual;
                timer = null;
                LimpiarItemsControl();
            }
            desechado = true;
        }

        private void Cerrado(object sender, RoutedEventArgs e)
        {
            Desechar();
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCargarAmigosFalla, Properties.Idioma.mensajeCargarAmigosFalla, this);
            }
            else
            {
                if (amigo.Estado != EstadoAmigo.Desconectado)
                {
                    amigo.UltimaConexion = "";
                }
                Amigos.Add(amigo);
            }
        }

        private void LimpiarItemsControl()
        {
            Amigos.Clear();
        }

        public void CambiarEstadoAmigo(Amigo amigo)
        {
            var amigoExistente = Amigos.FirstOrDefault(actual => actual.Nombre == amigo.Nombre);
            if (amigoExistente != null)
            {
                amigoExistente.Estado = amigo.Estado;
                amigoExistente.Foto = amigo.Foto;
                if (amigoExistente.Estado != EstadoAmigo.Desconectado)
                {
                    amigoExistente.UltimaConexion = "";
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloCargarAmigosFalla, 
                    Properties.Idioma.mensajeCargarAmigosFalla , 
                    this);
            }
        }

        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelListaAmigos.Content = Idioma.labelListaAmigos;
        }

        private void CerrandoUserControl(object sender, RoutedEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;

        }
    }
}
