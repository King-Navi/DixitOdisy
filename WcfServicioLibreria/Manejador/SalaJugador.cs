using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioSalaJugador
    {
        public bool ComenzarPartidaAnfrition(string nombre, string idSala , string idPartida)
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
            catch (Exception)
            {
                return false;
            };
        }
        /// <summary>
        /// Agrega a un jugador a una sala ya existente
        /// </summary>
        /// <param name="gamertag"></param>
        /// <param name="idSala"></param>
        public async Task AgregarJugadorSalaAsync(string gamertag, string idSala)
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
                    foreach (var nombreJugadorEnSala in sala.ObtenerNombresJugadoresSala())
                    {
                        if (nombreJugadorEnSala.Equals(gamertag, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                    sala.AgregarJugadorSala(gamertag, contexto);
                }
                await sala.AvisarNuevoJugador(gamertag);

            }
            catch (Exception)
            {
            }
        }

        public void ExpulsarJugadorSala(string anfitrion, string jugadorAExpulsar, string idSala)
        {
            if (!ValidarSala(idSala))
            {
                return;
            }
            try
            {
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                lock (sala)
                {
                    if (sala.Anfitrion.Equals(anfitrion, StringComparison.OrdinalIgnoreCase))
                    {
                        sala.DesconectarUsuario(jugadorAExpulsar);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
