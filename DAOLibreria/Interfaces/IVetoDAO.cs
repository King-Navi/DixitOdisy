using System;

namespace DAOLibreria.Interfaces
{
    public interface IVetoDAO
    {
        bool ExisteTablaVetoPorIdCuenta(int idUsuarioCuenta);
        bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente);
        bool VerificarVetoPorIdCuenta(int idUsuarioCuenta);
    }
}
