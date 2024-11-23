using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    internal class CondicionVictoriaCartasAgotadas : ICondicionVictoria
    {
        public bool Verificar(Partida partida)
        {
            bool resultado = false;
            if (partida.CartasRestantes <= 0)
            {
                resultado = true;
            };
            return resultado;
        }
    }
}
