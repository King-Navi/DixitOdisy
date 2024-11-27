using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Interfaz
{
    public interface IImplementacionCallback
    {
        bool AbrirConexion();
        bool CerrarConexion();
    }
}
