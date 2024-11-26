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
        /// <summary>
        /// Elimina una sala cuando ya no tiene usuarios.
        /// </summary>
        /// <param name="sender">La sala que desencadena el evento de ser eliminada.</param>
        /// <param name="e">Argumentos del evento, que contienen detalles específicos de la sala vacía.</param>
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
        /// <summary>
        /// Crea una nueva sala con un identificador único y la agrega al diccionario de salas activas.
        /// </summary>
        /// <param name="nombreUsuarioAnfitrion">El nombre del usuario que será el anfitrión de la sala.</param>
        /// <returns>El identificador único de la sala recién creada.</returns>
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
                    idSala = Utilidad.GenerarIdUnico();
                } while (salasDiccionario.ContainsKey(idSala));
                Sala salaNueva = new Sala(idSala, nombreUsuarioAnfitrion);
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
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }
            return idSala;
        }
        
        /// <summary>
        /// Valida si una sala con un identificador específico ya existe.
        /// </summary>
        /// <param name="idSala">El identificador de la sala a validar.</param>
        /// <returns>True si la sala existe, False en caso contrario.</returns>
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
            }catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return result;
        }
    }
}
