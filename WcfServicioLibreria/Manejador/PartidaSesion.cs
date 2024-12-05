using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartidaSesion
    {

        public async Task<bool> UnirsePartidaAsync(string usuarioNombre, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            try
            {
                IPartidaCallback contexto = contextoOperacion.GetCallbackChannel<IPartidaCallback>();
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                await partida.AgregarJugadorAsync(usuarioNombre, contexto);
                
                return true;
            }
            catch (FaultException<PartidaFalla> excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                throw;
            }
            catch (InvalidOperationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public void TratarAdivinar(string nombreJugador, string idPartida, string claveImagen)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                var narrador = partida.NarradorActual;
                if (!narrador.Equals(nombreJugador, StringComparison.OrdinalIgnoreCase))
                {
                    partida.ConfirmarTurnoAdivinarJugador(nombreJugador, claveImagen);

                }

            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public void ConfirmarMovimiento(string nombreJugador, string idPartida, string claveImagen, string pista = null)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public void ExpulsarJugadorPartida(string nombreJugador, string idPartida)
        {
            //TODO: 
            throw new NotImplementedException();
        }

        public async Task EmpezarPartidaAsync(string nombreJugador, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                await partida.EmpezarPartida();
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

    }
}
