using System.Windows;
using WpfCliente.Persistencia;

namespace WpfCliente
{
    public partial class App : Application
    {
        App()
        {
            IdiomaGuardo.CargarIdiomaGuardado();

        }
    }
}
