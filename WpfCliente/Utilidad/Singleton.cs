using System.Windows.Media.Imaging;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public sealed class Singleton
    {
        private static Singleton instance;
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
        private Singleton() { }
        public int IdUsuario { get; set; }
        public string IdSala { get; set; }
        public string IdChat { get; set; }
        public string NombreUsuario { get; set; }
        public string ContraniaHash { get; set; }
        public string Correo { get; set; }
        public BitmapImage FotoJugador { get; set; }
    }
}
