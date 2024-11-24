using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartidaSesion
    {

        public async Task UnirsePartida(string usuarioNombre, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                IPartidaCallback contexto = contextoOperacion.GetCallbackChannel<IPartidaCallback>();
                partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                partida.AgregarJugador(usuarioNombre, contexto);
                await partida.AvisarNuevoJugadorAsync(usuarioNombre);
                partida.ConfirmarInclusionPartida(contexto);
            }
            catch (Exception)
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

        public void ExpulsarJugadorPartida(string nombreJugador, string idPartida)
        {
            throw new NotImplementedException();
        }

        public async Task EmpezarPartida(string nombreJugador, string idPartida) 
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
