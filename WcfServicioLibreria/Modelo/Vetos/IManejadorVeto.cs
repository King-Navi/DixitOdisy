using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo.Vetos
{
    public interface IManejadorVeto
    {
        Task<bool> VetaJugadorAsync(string nombreJugador);
        Task<bool> RegistrarExpulsionJugadorAsync(string nombreJugador, string motivo, bool esHacker);

    }
}
