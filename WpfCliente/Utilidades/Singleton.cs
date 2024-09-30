using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public sealed class Singleton
    {
        private static Singleton _instance;
        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }
        }
        public string nombreUsuario { get; set; }
        public ServicioUsuarioSesionClient ServicioUsuarioSesionCliente { get; set; }
        public ServicioSalaJugadorClient ServicioSalaJugadorCliente { get; set; }
        public string idSala {  get; set; }
        public string idChat { get; set; }

    }
}
