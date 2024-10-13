using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Utilidades
{
    public interface IObservador
    {
        void DesconectarUsuario(string clave);
    }
}
