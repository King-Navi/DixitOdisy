using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Interfaces
{
    public interface IVetoDAO
    {
        bool ExisteTablaVetoPorIdCuenta(int idUsuarioCuenta);
        bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente);
        bool VerificarVetoPorIdCuenta(int idUsuarioCuenta);
    }
}
