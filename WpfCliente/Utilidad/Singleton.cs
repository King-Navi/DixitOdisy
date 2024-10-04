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

        public string NombreUsuario { get; set; }
        public string IdSala {  get; set; }
        public string IdChat { get; set; }
    }
}
