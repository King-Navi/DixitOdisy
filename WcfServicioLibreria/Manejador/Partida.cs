using DAOLibreria.DAO;
using System;
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
                    idPartida = Utilidad.Generar6Caracteres();
                } while (salasDiccionario.ContainsKey(idPartida));
                MediadorPartida medidador = new MediadorPartida(configuracion.Tematica);
                Partida partidaNueva = new Partida(idPartida, anfitrion, configuracion, new EstadisticasDAO(), medidador);
                ManejadorImagen manejadorImagen = new ManejadorImagen(Escritor ,medidador, configuracion.Tematica);
                partidaNueva.MostrarTodasLasCartas += manejadorImagen.EnMostrarImagenes;
                partidaNueva.PartidaVaciaManejadorEvento += manejadorImagen.SeTerminoLectura;
                bool existeSala = partidasDiccionario.TryAdd(idPartida, partidaNueva);
                bool existeManejador = manejadoresImagenes.TryAdd(idPartida, manejadorImagen);
                if (existeSala && existeManejador)
                {
                    partidaNueva.PartidaVaciaManejadorEvento += BorrarPartida;
                }
                else
                {
                    partidasDiccionario.TryRemove(idPartida, out _);
                    manejadoresImagenes.TryRemove(idPartida, out _);
                    throw new Exception("No se creo la Partida");
                }
            }
            catch (Exception excepcion)
            {
                partidasDiccionario.TryRemove(idPartida, out _);
                manejadoresImagenes.TryRemove(idPartida, out _);
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return idPartida;
        }

        public void BorrarPartida(object sender, EventArgs e)
        {
            if (sender is Partida partida)
            {
                PartidaVaciaEventArgs evento = e as PartidaVaciaEventArgs;
                partida.PartidaVaciaManejadorEvento -= BorrarSala;
                partidasDiccionario.TryRemove(evento.Partida.IdPartida, out _);
                Console.WriteLine($"La partdia con ID {evento.Partida.IdPartida} está vacía y será eliminada.");
            };
        }

        public bool ValidarPartida(string idPartida)
        {
            if (idPartida == null)
            {
                return false;
            }
            bool result = partidasDiccionario.ContainsKey(idPartida);
            return result;
        }

        public bool EsPartidaEmpezada(string idPartida)
        {
            bool resultado = false;
            if (ValidarPartida(idPartida))
            {
                try
                {
                    partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                    resultado = partida.SeLlamoEmpezarPartida;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excepcion);
                }
            }
            return resultado;
        }
    }
}
