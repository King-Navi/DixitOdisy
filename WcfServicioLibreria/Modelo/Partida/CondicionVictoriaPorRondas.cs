using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    internal class CondicionVictoriaPorRondas : ICondicionVictoria
    {
        private int NumeroRondasParaFinalizar;
        public CondicionVictoriaPorRondas(int numRondas) 
        {
            NumeroRondasParaFinalizar = numRondas;
        }
        public bool Verificar(Partida partida)
        {
            bool resultado =false;
            if (partida.RondaActual > NumeroRondasParaFinalizar)
            {
                resultado = true;
            }
            return resultado;
        }
    }
}
