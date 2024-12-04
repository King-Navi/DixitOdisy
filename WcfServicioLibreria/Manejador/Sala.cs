using DAOLibreria.DAO;
using System;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioSala
    {
        internal void BorrarSala(object sender, EventArgs e)
        {
            try
            {
                if (sender is Sala sala)
                {
                    SalaVaciaEventArgs evento = e as SalaVaciaEventArgs;
                    sala.salaVaciaManejadorEvento -= BorrarSala;
                    salasDiccionario.TryRemove(evento.Sala.IdCodigoSala, out _);
                    Console.WriteLine($"La sala con ID {evento.Sala.IdCodigoSala} está vacía y será eliminada.");
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
        }
       
        public string CrearSala(string nombreUsuarioAnfitrion)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuarioAnfitrion))
            {
                return null;
            }
            string idSala = null;
            try
            {
                do
                {
                    idSala = Utilidad.Generar6Caracteres();
                } while (salasDiccionario.ContainsKey(idSala));
                Sala salaNueva = new Sala(idSala, nombreUsuarioAnfitrion, new UsuarioDAO());
                bool existeSala = salasDiccionario.TryAdd(idSala, salaNueva);
                if (existeSala)
                {
                    salaNueva.salaVaciaManejadorEvento += BorrarSala;
                }
                else
                {
                    throw new Exception("No se creo la sala");
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return idSala;
        }
        
        public bool ValidarSala(string idSala)
        {
            bool result = false;
            try
            {
                result = salasDiccionario.ContainsKey(idSala);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatal(excepcion);
            }
            return result;
        }
    }
}
