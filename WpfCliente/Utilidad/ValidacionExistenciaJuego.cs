using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.GUI;
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
