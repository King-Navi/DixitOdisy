using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public interface IObservadorSala
    {
        void Desconectar(string clave);
    }
}
