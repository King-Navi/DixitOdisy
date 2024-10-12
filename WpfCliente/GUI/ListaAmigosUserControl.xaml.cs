using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Interaction logic for ListaAmigosUserControl.xaml
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-8.0</ref>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/desktop/wpf/data/?view=netdesktop-8.0</ref>
    public partial class ListaAmigosUserControl : UserControl , IServicioPeticionAmistadCallback
    {
        //Aqui se deberia colocar el modelo friend
        public ObservableCollection<Amigo> Amigos { get; set; }

        public ListaAmigosUserControl()
        {
            InitializeComponent();
            // TODO: Remplazar esto por una llamada al servidor para obtener los amigos
            Amigos = new ObservableCollection<Amigo>
            {
                new Amigo { Nombre = "Juan", Foto = null },
                new Amigo { Nombre = "Ana" , Foto = null},
                new Amigo { Nombre = "Carlos", Foto = null}
            };
            DataContext = this;
            
        }

        bool IServicioPeticionAmistadCallback.ObtenerPeticionAmistadCallback(SolicitudAmistad nuevaSolicitudAmistad)
        {
            throw new NotImplementedException();
        }
    }
}
