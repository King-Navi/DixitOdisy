using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartida
    {
        public string CrearPartida(string anfitrion, ConfiguracionPartida configuracion)
        {
            if (anfitrion == null || configuracion == null)
            {
                return null;
            }
            string idPartida = null;
            try
            {
                do
                {
                    idPartida = Utilidad.GenerarIdUnico();
                } while (salasDiccionario.ContainsKey(idPartida));
                Partida partidaNueva = new Partida(idPartida, anfitrion, configuracion, Escritor);
                bool existeSala = partidasdDiccionario.TryAdd(idPartida, partidaNueva);
                if (existeSala)
                {
                    partidaNueva.partidaVaciaManejadorEvento += BorrarPartida;
                }
                else
                {
                    throw new Exception("No se creo la Partida");
                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }

            return idPartida;
        }

        public void BorrarPartida(object sender, EventArgs e)
        {
            if (sender is Partida partida)
            {
                PartidaVaciaEventArgs evento = e as PartidaVaciaEventArgs;
                partida.partidaVaciaManejadorEvento -= BorrarSala;
                partidasdDiccionario.TryRemove(evento.Partida.IdPartida, out _);
                Console.WriteLine($"La partdia con ID {evento.Partida.IdPartida} está vacía y será eliminada.");
            };
        }

        public bool ValidarPartida(string idPartida)
        {
            if (idPartida == null)
            {
                return false;
            }
            bool result = partidasdDiccionario.ContainsKey(idPartida);
            return result;
        }

        public bool EsPartidaEmpezada(string idPartida)
        {
            bool resultado = false;
            if (ValidarPartida(idPartida))
            {
                try
                {
                    lock (partidasdDiccionario)
                    {
                        partidasdDiccionario.TryGetValue(idPartida, out Partida partida);
                        resultado = partida.PartidaEnProgreso;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
            return resultado;
        }
    }
}
