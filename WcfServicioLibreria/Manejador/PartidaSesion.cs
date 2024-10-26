using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartidaSesion
    {
        public void AvanzarRonda()
        {
            throw new NotImplementedException();
        }
        public void UniserPartida(string gamertag, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                IPartidaCallback contexto = contextoOperacion.GetCallbackChannel<IPartidaCallback>();
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                lock (partida)
                {
                    partida.AgregarJugador(gamertag, contexto);
                    //TODO: partida.AvisarNuevoJugador(gamertag);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            };
        }

        public void ConfirmarMovimiento()
        {
            throw new NotImplementedException();
        }

        public void EnviarImagenCarta()
        {
            throw new NotImplementedException();
        }

        public void ExpulsarJugador()
        {
            throw new NotImplementedException();
        }

        public void FinalizarPartida()
        {
            throw new NotImplementedException();
        }
    }
}
