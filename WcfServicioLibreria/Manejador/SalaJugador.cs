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
        public void AgregarJugadorSala(string gamertag, string idSala)
        {
            if (!ValidarSala(idSala))
            {
                return;
            }
            try
            {
                ISalaJugadorCallback contexto = contextoOperacion.GetCallbackChannel<ISalaJugadorCallback>();
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                lock (sala)
                {
                    sala.AgregarJugadorSala(gamertag, contexto);
                    sala.AvisarNuevoJugador(gamertag);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }
        }

        public void AsignarColor(string idSala)
        {
            throw new NotImplementedException();
        }

        public void EmpezarPartida(string idSala)
        {
            throw new NotImplementedException();
        }
    }
}
