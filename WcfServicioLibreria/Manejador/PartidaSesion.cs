using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Modelo.Vetos;
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
            if (String.IsNullOrEmpty(nombreJugador))
            {
                return;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
        }

        public async Task<bool> ExpulsarJugadorPartida(string anfitrion, string jugadorAExpulsar, string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            if (anfitrion.Equals(jugadorAExpulsar, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Modelo.Partida partida);
                if (String.IsNullOrEmpty(anfitrion))
                {
                    return false;
                }
                if (partida.Anfitrion.Equals(anfitrion, StringComparison.OrdinalIgnoreCase))
                {
                    ManejadorDeVetos veto = new ManejadorDeVetos();
                    var resultado = await veto.RegistrarExpulsionJugadorAsync(jugadorAExpulsar, MOTIVO_EXPULSION_SALA, false);
                    lock (partida)
                    {
                        partida.DesconectarUsuario(jugadorAExpulsar);
                    }
                    return true;
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public async Task EmpezarPartidaAsync(string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return;
            }
            try
            {
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                if (partida.SeLlamoEmpezarPartida)
                {
                    return;
                }
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
