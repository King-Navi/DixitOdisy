using System.Windows.Media.Imaging;

namespace WpfCliente.Utilidad
{
    public sealed class SingletonCliente
    {
        private static SingletonCliente instance;
        public static SingletonCliente Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingletonCliente();
                }
                return instance;
            }
        }
        private SingletonCliente() { }
        public int IdUsuario { get; set; }
        public string IdSala { get; set; }
        public string IdChat { get; set; }
        public string IdPartida { get; set; }
        public string NombreUsuario { get; set; }
        public string ContraniaHash { get; set; }
        public string Correo { get; set; }
        public BitmapImage FotoJugador { get; set; }
    }
}
