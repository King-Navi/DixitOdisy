using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
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
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }

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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
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
                if (String.IsNullOrEmpty(anfitrion))
                {
                    return false;
                }
                if (sala.Anfitrion.Equals(anfitrion, StringComparison.OrdinalIgnoreCase))
                {
                    ManejadorDeVetos veto = new ManejadorDeVetos();
                    var resultado = await veto.RegistrarExpulsionJugadorAsync(jugadorAExpulsar, MOTIVO_EXPULSION_SALA, false);
                    lock (sala)
                    {
                        sala.DesconectarUsuario(jugadorAExpulsar);
                    }
                    return true;
                }

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }
    }
}
