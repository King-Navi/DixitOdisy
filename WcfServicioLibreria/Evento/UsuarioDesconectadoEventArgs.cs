using System;

namespace WcfServicioLibreria.Evento
{
    public class UsuarioDesconectadoEventArgs: EventArgs
    {
        public DateTime DesconetadoHoraFecha { get; set; }
        public string NombreUsuario { get; set; }
        public UsuarioDesconectadoEventArgs(string nombreUsuario, DateTime desconetadoHoraFecha)
        {
            DesconetadoHoraFecha = desconetadoHoraFecha;
            NombreUsuario = nombreUsuario;
        }
    }
}
