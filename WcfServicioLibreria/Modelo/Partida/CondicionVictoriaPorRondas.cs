using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    internal class CondicionVictoriaPorRondas : ICondicionVictoria
    {
        private int NumeroRondasParaFinalizar;
        public CondicionVictoriaPorRondas(int numeroRondas) 
        {
            NumeroRondasParaFinalizar = numeroRondas;
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
