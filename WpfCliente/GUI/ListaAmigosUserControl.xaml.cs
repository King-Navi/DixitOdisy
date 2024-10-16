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
    public partial class ListaAmigosUserControl : UserControl, IServicioAmistadCallback
    {
        //Aqui se deberia colocar el modelo friend
        private ObservableCollection<Amigo> amigos;
        private bool desechado = false;
        private DispatcherTimer timer;
        private DateTime ultimaActualizacion;
        public ListaAmigosUserControl()
        {
            InitializeComponent();
            IniciarHora();
            DataContext = this;

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

        public void ObtenerListaAmigoCallback(Amigo[] amigos)
        {
            if (amigos == null)
            {
                //TODO: manejar que pasa si no se pueden cargar datos o no tiene amigos
                MessageBox.Show("No se pudo cargar los datos de tus amigos");
            }
            LimpiarItemsControl();
            AgregarAmigos(amigos);
        }

        private void AgregarAmigos(IEnumerable<Amigo> amigos)
        {
            //Fixme Verificar que datos se deben poner
            foreach (var amigo in amigos)
            {
                var amigoControl = new AmigoUserControl
                {
                    DataContext = amigo
                };
                itemsControlAmigos.Items.Add(amigoControl);
            }

            for (int i = 0; i < 20; i++)
            {
                itemsControlAmigos.Items.Add(new AmigoUserControl("Nombre inventado", "Haciendo algo") );

            }


        }
        private void LimpiarItemsControl()
        {
            itemsControlAmigos.Items.Clear();
        }
    }
}
