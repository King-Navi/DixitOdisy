using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    public interface ISala
    {
        string IdCodigoSala { get; }
        string Anfitrion { get; }
        bool EsVacia();
        IReadOnlyCollection<string> ObtenerNombresJugadoresSala();
        bool AgregarJugadorSala(string nombreJugador, ISalaJugadorCallback nuevoContexto);
        bool RemoverJugadorSala(string nombreJugador);
        bool DelegarRolAnfitrion(string nuevoAnfitrionNombre);
    }
}
