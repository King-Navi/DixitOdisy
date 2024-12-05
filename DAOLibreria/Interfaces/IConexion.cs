using System.Threading.Tasks;

namespace DAOLibreria.Interfaces
{
    public interface IConexion
    {
        Task<bool> VerificarConexionAsync();
        bool VerificarConexion();
    }
}
