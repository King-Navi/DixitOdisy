using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    public class UsuarioConectadoEventArgs : EventArgs
    {
        public DateTime DesconetadoHoraFecha { get; set; }
        public string NombreUsuario { get; set; }
        public int IdUsuario { get; set; }
        public UsuarioConectadoEventArgs(string nombreUsuario, DateTime desconetadoHoraFecha)
        {
            DesconetadoHoraFecha = desconetadoHoraFecha;
            NombreUsuario = nombreUsuario;
        }
        public UsuarioConectadoEventArgs(string nombreUsuario, DateTime desconetadoHoraFecha, int idUsuario)
        {
            DesconetadoHoraFecha = desconetadoHoraFecha;
            NombreUsuario = nombreUsuario;
            IdUsuario = idUsuario;
        }
    }
}
