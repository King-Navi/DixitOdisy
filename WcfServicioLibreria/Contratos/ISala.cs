using System.Collections.Generic;

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
