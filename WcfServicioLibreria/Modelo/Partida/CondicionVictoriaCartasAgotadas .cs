using System.Runtime.Serialization;

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
