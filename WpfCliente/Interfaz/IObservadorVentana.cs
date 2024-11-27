using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.Contexto;

namespace WpfCliente.Interfaz
{
    public interface IObservadorVentana
    {
        void EnCierre(Ventana nombre);
    }
}
