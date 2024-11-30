using System.Collections.ObjectModel;
using System.Linq;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.GUI
{
    internal class RecursosCompartidosPartida
    {
        public Usuario primerLugar;
        public Usuario segundoLugar;
        public Usuario tercerLugar;
        public ObservableCollection<ImagenCarta> GruposDeImagenes { get; } = new ObservableCollection<ImagenCarta>();
        public ObservableCollection<ImagenCarta> Imagenes { get; } = new ObservableCollection<ImagenCarta>();
        public ObservableCollection<JugadorEstadisticas> JugadorEstadisticas { get; set; } = new ObservableCollection<JugadorEstadisticas>();
        public ObservableCollection<Usuario> UsuarioEnpartida { get; set; } = new ObservableCollection<Usuario>();
        public ObservableCollection<string> Podio { get; set; } = new ObservableCollection<string>();


        public RecursosCompartidosPartida()
        {
            Podio.Add("");
            Podio.Add("");
            Podio.Add("");
        }

        public void AsignarPodio(JugadorEstadisticas _primerLugar, JugadorEstadisticas _segundoLugar, JugadorEstadisticas _tercerLugar)
        {
            primerLugar = _primerLugar != null
                ? UsuarioEnpartida.FirstOrDefault(usuario => usuario.Nombre == _primerLugar.Nombre)
                : null;

            segundoLugar = _segundoLugar != null
                ? UsuarioEnpartida.FirstOrDefault(usuario => usuario.Nombre == _segundoLugar.Nombre)
                : null;

            tercerLugar = _tercerLugar != null
                ? UsuarioEnpartida.FirstOrDefault(usuario => usuario.Nombre == _tercerLugar.Nombre)
                : null;
        }

        public void EliminarUsuarioSala(Usuario usuario)
        {
            var usuarioAEliminar = UsuarioEnpartida.FirstOrDefault(busqueda => busqueda.Nombre == usuario.Nombre);
            if (usuarioAEliminar != null)
            {
                UsuarioEnpartida.Remove(usuarioAEliminar);
            }
        }


        public void ObtenerUsuarioSala(Usuario usuario)
        {
            if (usuario == null)
            {
                return;
            }
            else
            {
                UsuarioEnpartida.Add(usuario);
            }
        }
    }
}