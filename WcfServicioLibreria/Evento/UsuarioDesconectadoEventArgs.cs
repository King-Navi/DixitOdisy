using System;

namespace WcfServicioLibreria.Evento
{
    public class UsuarioDesconectadoEventArgs: EventArgs
    {
        public DateTime DesconetadoHoraFecha { get; set; }
        public string NombreUsuario { get; set; }
        public int IdUsuario { get; set; }
        public UsuarioDesconectadoEventArgs(string nombreUsuario, DateTime desconetadoHoraFecha)
        {
            DesconetadoHoraFecha = desconetadoHoraFecha;
            NombreUsuario = nombreUsuario;
        }
        public UsuarioDesconectadoEventArgs(string nombreUsuario, DateTime desconetadoHoraFecha, int idUsuario)
        {
            DesconetadoHoraFecha = desconetadoHoraFecha;
            NombreUsuario = nombreUsuario;
            IdUsuario = idUsuario;
        }
    }
}
