using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartidaSesion
    {
        public void UnirsePartida(string gamertag, string idPartida)
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
                    partida.AvisarNuevoJugador(gamertag);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            };
        }

        public void ConfirmarMovimiento(string nombreJugador, string idPartida, string claveImagen, string pista = null)
        {
            if (!ValidarPartida(idPartida))
            {
                throw new FaultException<PartidaFalla>(new PartidaFalla() { PartidaInvalida = true}, new FaultReason("El ID de la partida es invalido"));
            }
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                lock (partida)
                {
                    var narrador = partida.NarradorActual;
                    if (narrador == nombreJugador && pista != null)
                    {
                        partida.ConfirmacionTurnoNarrador(nombreJugador, claveImagen, pista);

                    }
                    else
                    {
                        partida.ConfirmacionTurnoJugador(nombreJugador, claveImagen);
                    }
                }
            }
            catch (Exception excepcion)
            {
            };
        }


        public void ExpulsarJugador(string nombreJugador, string idPartida)
        {
            throw new NotImplementedException();
        }
        public async Task EmpezarPartida(string nombreJugador, string idPartida) //FIXME: ¿La mehor manera de empezar la partida?
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                await partida.EmpezarPartida();
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            };
        }

        public async Task SolicitarImagenCartaAsync(string nombreJugador, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                await partida.EnviarImagen(nombreJugador);
            }
            catch (Exception excepcion)
            {
            };
        }
    }
}
