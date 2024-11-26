using System.Windows;
using WpfCliente.Contexto;
using WpfCliente.GUI;
using WpfCliente.Persistencia;

namespace WpfCliente
{
    public partial class App : Application
    {
        App()
        {
            IdiomaGuardo.CargarIdiomaGuardado();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SingletonGestorVentana.Instancia.Iniciar();
        }
    }
}
