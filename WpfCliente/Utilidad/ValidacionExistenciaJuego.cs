using System;
using WpfCliente.ServidorDescribelo;

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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return false;
            }
        }

    }
}
