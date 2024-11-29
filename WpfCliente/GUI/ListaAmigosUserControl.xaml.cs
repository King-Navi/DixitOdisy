using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    public partial class ListaAmigosUserControl : UserControl, IServicioAmistadCallback, IActualizacionUI
    {
        public ObservableCollection<Amigo> Amigos { get; set; } = new ObservableCollection<Amigo>();
        private bool desechado = false;
        private DispatcherTimer timer;
        private DateTime ultimaActualizacion;
        private const string FORMATO_HORA = "HH:mm:ss";
        private const int VALOR_PARA_INTERVALO = 500;
        private const string UTILIMA_CONEXION_CONECTADO = "";
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
            timer.Interval = TimeSpan.FromMilliseconds(VALOR_PARA_INTERVALO);
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
                VentanasEmergentes.CrearVentanaEmergente(Properties.Idioma.tituloCargarAmigosFalla,
                    Properties.Idioma.mensajeCargarAmigosFalla, this);
            }
            else
            {
                if (amigo.Estado == EstadoAmigo.Desconectado)
                {
                    amigo.EstadoActual = Properties.Idioma.labelDesconectado;
                }
                if (amigo.Estado == EstadoAmigo.Conectado)
                {
                    amigo.EstadoActual = Properties.Idioma.labelConectado;
                    amigo.UltimaConexion = UTILIMA_CONEXION_CONECTADO;

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
            amigo.BitmapImagen = Imagen.ConvertirStreamABitmapImagen(amigo.Foto);
            if (amigo != null)
            {
                if (amigo.Estado == EstadoAmigo.Conectado)
                {
                    var amigoAEliminar = Amigos.FirstOrDefault(busqueda =>
                        busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));
                    if (amigoAEliminar != null)
                    {
                        Amigos.Remove(amigoAEliminar);
                        amigo.UltimaConexion = UTILIMA_CONEXION_CONECTADO;
                        amigo.EstadoActual = Properties.Idioma.labelConectado;
                        Amigos.Insert(0, amigo);
                    }
                }
                else
                {
                    var amigoAEliminar = Amigos.FirstOrDefault(busqueda =>
                        busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));
                    if (amigoAEliminar != null)
                    {
                        Amigos.Remove(amigoAEliminar);
                        amigo.EstadoActual = Properties.Idioma.labelDesconectado;
                        Amigos.Insert(0, amigo);
                    }
                }
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

        public void EliminarAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                return;
            }
            var amigoAEliminar = Amigos.FirstOrDefault(busqueda =>
                busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));

            if (amigoAEliminar != null)
            {
                Amigos.Remove(amigoAEliminar);
            }
        }

        private void OnAmigoEliminado(object sender, Amigo amigo)
        {
            if (amigo == null) return;

            // Buscar y eliminar al amigo de la lista
            var amigoAEliminar = Amigos.FirstOrDefault(a => a.Nombre == amigo.Nombre);
            if (amigoAEliminar != null)
            {
                Amigos.Remove(amigoAEliminar);
            }
        }
    }
}
