using DAOLibreria.DAO;
using System;
using System.Runtime.InteropServices;
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
            if (sender is Sala sala)
            {
                SalaVaciaEventArgs evento = e as SalaVaciaEventArgs;
                sala.salaVaciaManejadorEvento -= BorrarSala;
                salasDiccionario.TryRemove(evento.Sala.IdCodigoSala, out _);    
                Console.WriteLine($"La sala con ID {evento.Sala.IdCodigoSala} está vacía y será eliminada.");
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
                ManejadorExcepciones.ManejarFatalException(excepcion);
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
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarFatalException(excepcion);
            }
            return result;
        }
    }
}
