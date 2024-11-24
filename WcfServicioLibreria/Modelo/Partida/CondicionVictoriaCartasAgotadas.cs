using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    internal class CondicionVictoriaCartasAgotadas : ICondicionVictoria
    {
        private const int CARTA_CERO = 0;
        public bool Verificar(Partida partida)
        {
            bool resultado = false;
            if (partida.CartasRestantes <= CARTA_CERO)
            {
                resultado = true;
            };
            return resultado;
        }
    }
}
