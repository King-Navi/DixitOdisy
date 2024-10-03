using System;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioSalaJugador
    {
        /// <summary>
        /// Agrega a un jugador a una sala ya existente
        /// </summary>
        /// <param name="gamertag"></param>
        /// <param name="idSala"></param>
        public bool AgregarJugadorSala(string gamertag, string idSala)
        {
            if (!ValidarSala(idSala))
            {
                return false;
            }
            try
            {
                ISalaJugadorCallback contexto = OperationContext.Current.GetCallbackChannel<ISalaJugadorCallback>();
                salasDiccionario.TryGetValue(idSala, out ISala sala);
                lock (sala)
                {
                    sala.AgregarJugadorSala(gamertag, contexto);

                    ICommunicationObject comunicacionObjecto = (ICommunicationObject)contexto;
                    comunicacionObjecto.Closing += (emisor, e) =>
                    {
                        Console.WriteLine(emisor + "Se esta llendo de la sala");////
                    };
                    comunicacionObjecto.Closed += (emisor, e) =>
                    {
                        Console.WriteLine(emisor + "Se ha ido de la sala"); ///

                    };

                    comunicacionObjecto.Faulted += (emisor, e) =>
                    {
                        Console.WriteLine(emisor + "Ha fallado de la sala"); ////

                    };
                }

            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }
            return true;
        }

        public void AsignarColor(string idSala)
        {
            throw new NotImplementedException();
        }

        public void EmpezarPartida(string idSala)
        {
            throw new NotImplementedException();
        }

        public void ObtenerJugadoresSala(string gamertag, string idSala)
        {
            throw new NotImplementedException();
        }

        public void RemoverJugadorSala(string gamertag, string ididSalaRoom)
        {
            throw new NotImplementedException();
        }
    }
}
