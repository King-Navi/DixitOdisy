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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ListaAmigosUserControl.xaml
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-8.0</ref>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/desktop/wpf/data/?view=netdesktop-8.0</ref>
    public partial class ListaAmigosUserControl : UserControl, IServicioAmistadCallback , IActualizacionUI
    {
        public ObservableCollection<Amigo> Amigos { get; set; } = new ObservableCollection<Amigo>();
        private bool desechado = false;
        private DispatcherTimer timer;
        private DateTime ultimaActualizacion;
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
        private void HoraActual(object sender, EventArgs e)
        {
            DateTime horaActual = DateTime.Now;

            // Compara la hora actual con la última actualización y actualiza solo si ha cambiado el segundo
            if (horaActual.Second != ultimaActualizacion.Second)
            {
                labelHora.Content = horaActual.ToString("HH:mm:ss");
                ultimaActualizacion = horaActual;
            }
        }

        protected virtual void Desechar()
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

        public void ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad)
        {
            throw new NotImplementedException();
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                MessageBox.Show("No se pudo cargar los datos de tus amigos");
            }
            else
            {
                // Agregar el amigo a la colección ObservableCollection, esto actualizará automáticamente el ItemsControl
                Amigos.Add(amigo);
            }
        }

        private void LimpiarItemsControl()
        {
            Amigos.Clear();
        }

        public void CambiarEstadoAmigo(Amigo amigo)
        {
            var amigoExistente = Amigos.FirstOrDefault(a => a.Nombre == amigo.Nombre);
            if (amigoExistente != null)
            {
                // Actualizar las propiedades del amigo encontrado
                amigoExistente.Estado = amigo.Estado;
                amigoExistente.Foto = amigo.Foto;

                // más propiedades que actualizar aquí

                // Notificar cambios en la colección
                // Si la clase Amigo implementa INotifyPropertyChanged, el ItemsControl se actualizará automáticamente. (Amigo es partial de WCF y parece que a la implemeta ¿¿??)
            }
            else
            {
                MessageBox.Show("El amigo no se encontró en la lista.");
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
