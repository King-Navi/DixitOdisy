using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    internal class CondicionVictoriaCartasAgotadas : ICondicionVictoria
    {
        public bool Verificar(Partida partida)
        {
            bool resultado = false;
            //TODO : hacer la logica para sbaer que pasa si las cartas se agutaron
            if (partida.CartasRestantes <= 0)
            {
                resultado = true;
            };
            return resultado;
        }
    }
}
