using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public interface IObservador
    {
        Task DesconectarUsuarioAsync(string nombreJugador);
    }
}
