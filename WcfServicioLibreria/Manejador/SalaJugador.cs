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
                ISalaJugadorCallback contexto = contextoOperacion.GetCallbackChannel<ISalaJugadorCallback>();
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                lock (sala)
                {
                    //FIXME
                    sala.AgregarJugadorSala(gamertag, contexto);

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
                    //FIXME

                    sala.ObtenerNombresJugadoresSala();

                }

            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }
        }

        public void RemoverJugadorSala(string gamertag, string ididSalaRoom)
        {
            throw new NotImplementedException();
        }
    }
}
