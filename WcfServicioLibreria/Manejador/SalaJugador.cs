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
    public partial class ManejadorPrincipal : IServicioSalaJugador
    {
        public bool ComenzarPartidaAnfrition(string nombre, string idSala, string idPartida)
        {
            var resultado = false;
            try
            {
                salasDiccionario.TryGetValue(idSala, out Sala sala);
                lock (sala)
                {
                    resultado = sala.AvisarComienzoPatida(nombre, idPartida);
                }
                return resultado;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public async Task<bool> AgregarJugadorSalaAsync(string gamertag, string idSala)
        {

            if (!ValidarSala(idSala))
            {
                return false;
            }
            try
            {
                ISalaJugadorCallback contexto = contextoOperacion.GetCallbackChannel<ISalaJugadorCallback>();
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                var cantidadJugadores = sala.ObtenerNombresJugadoresSala().Count;
                int disponible = sala.sePuedeUnir;
                EvaluarDisponibilidad(cantidadJugadores, disponible);
                foreach (var nombreJugadorEnSala in sala.ObtenerNombresJugadoresSala())
                {
                    if (nombreJugadorEnSala.Equals(gamertag, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                sala.AgregarInformacionJugadorSala(gamertag, contexto);
                await sala.AvisarNuevoJugador(gamertag);
                return true;
            }
            catch (FaultException<SalaFalla> excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                throw new FaultException<SalaFalla>(new SalaFalla()
                {
                    EstaLlena = true
                });
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        private void EvaluarDisponibilidad(int cantidadJugadores, int disponible)
        {
            if (cantidadJugadores >= Sala.CANTIDAD_MAXIMA_JUGADORES)
            {
                throw new FaultException<SalaFalla>(new SalaFalla()
                {
                    EstaLlena = true
                });
            }
            if (disponible == Sala.NO_UNISER)
            {
                throw new FaultException<SalaFalla>(new SalaFalla()
                {
                    EstaLlena = true
                });
            }
        }

        public async Task<bool> ExpulsarJugadorSalaAsync(string anfitrion, string jugadorAExpulsar, string idSala)
        {
            if (!ValidarSala(idSala))
            {
                return false;
            }
            if (anfitrion.Equals(jugadorAExpulsar, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            try
            {
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                if (string.IsNullOrEmpty(anfitrion))
                {
                    return false;
                }
                if (sala.Anfitrion.Equals(anfitrion, StringComparison.OrdinalIgnoreCase))
                {
                    ManejadorDeVetos veto = new ManejadorDeVetos();
                    var resultado = await veto.RegistrarExpulsionJugadorAsync(jugadorAExpulsar, MOTIVO_EXPULSION_SALA, false);

                    await sala.DesconectarUsuarioAsync(jugadorAExpulsar);

                    return true;
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }
    }
}
