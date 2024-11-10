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

        public async Task UnirsePartida(string gamertag, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                IPartidaCallback contexto = contextoOperacion.GetCallbackChannel<IPartidaCallback>();
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                if (partida.SeTerminoEsperaUnirse) //FIXME: Revisar si funciona
                {
                    return;
                }
                partida.AgregarJugador(gamertag, contexto);
                await partida.AvisarNuevoJugadorAsync(gamertag);
                partida.ConfirmarInclusionPartida(gamertag, contexto);
            }
            catch (Exception)
            {
                //TODO: Manejar el error
                Console.WriteLine("Error en el metodo  UnirsePartida(string gamertag, string idPartida)");
            }
            finally
            {
            }
        }

        public void TratarAdivinar(string nombreJugador, string idPartida, string claveImagen)
        {
            if (!ValidarPartida(idPartida)) 
            { 
                return;
            }
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                lock (partida)
                {
                    var narrador = partida.NarradorActual;
                    if (narrador != nombreJugador)
                    {
                        partida.ConfirmarTurnoAdivinarJugador(nombreJugador, claveImagen);

                    }
                }
            }
            catch (Exception)
            {
            };
        }

        public void ConfirmarMovimiento(string nombreJugador, string idPartida, string claveImagen, string pista = null)
        {
            if (!ValidarPartida(idPartida)) //
            {
                return;
            }
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                lock (partida)
                {
                    var narrador = partida.NarradorActual;
                    if (narrador == nombreJugador && pista != null)
                    {
                        partida.ConfirmarTurnoNarrador(nombreJugador, claveImagen, pista);

                    }
                    else
                    {
                        partida.ConfirmacionTurnoEleccionJugador(nombreJugador, claveImagen);
                    }
                }
            }
            catch (Exception)
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
            catch (Exception)
            {
                //TODO: Manejar el error
            };
        }

        public async Task<bool> SolicitarImagenCartaAsync(string nombreJugador, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            } 
            try
            {
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                return await partida.EnviarImagen(nombreJugador);
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
