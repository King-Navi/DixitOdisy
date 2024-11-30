using System.Collections.Concurrent;
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
        private readonly ConcurrentQueue<ImagenCarta> imagenesGrupoPendientes = new ConcurrentQueue<ImagenCarta>();
        public ObservableCollection<ImagenCarta> GruposDeImagenes { get; } = new ObservableCollection<ImagenCarta>();
        private readonly ConcurrentQueue<ImagenCarta> imagenesPendientes = new ConcurrentQueue<ImagenCarta>();
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
        public void RecibirImagen(ImagenCarta imagen)
        {
            imagenesPendientes.Enqueue(imagen);
            ProcesarImagenesPendientes();
        }

        private void ProcesarImagenesPendientes()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                while (imagenesPendientes.TryDequeue(out var imagen))
                {
                    Imagenes.Add(imagen);
                }
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(ProcesarImagenesPendientes);
            }
        }
        public void RecibirGrupoImagen(ImagenCarta imagen)
        {
            imagenesGrupoPendientes.Enqueue(imagen);
            ProcesarGrupoImagenesPendientes();
        }
        private void ProcesarGrupoImagenesPendientes()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                while (imagenesGrupoPendientes.TryDequeue(out var imagen))
                {
                    GruposDeImagenes.Add(imagen);
                }
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(ProcesarGrupoImagenesPendientes);
            }
        }
    }
}