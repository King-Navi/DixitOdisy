using System;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente
{
    public class ValidacionExistenciaJuego
    {
        public static bool ExisteSala(string codigoSala)
        {
            try
            {
                IServicioSala servicioSala = new ServicioSalaClient();
                return servicioSala.ValidarSala(codigoSala);
            }
            catch (EndpointNotFoundException enndpointException)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(enndpointException);
                return false;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                return false;
            }
        }
        public static bool ExistePartida(string codigoPartida)
        {
            try
            {
                IServicioPartida servicioPartida = new ServicioPartidaClient();
                return servicioPartida.ValidarPartida(codigoPartida);
            }
            catch (EndpointNotFoundException enndpointException)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(enndpointException);
                return false;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                return false;
            }
        }

    }
}
