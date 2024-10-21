using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{

    public interface ICondicionVictoria
    {
        bool Verificar(Partida partida);
    }
}
