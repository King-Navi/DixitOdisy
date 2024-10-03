using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.Utilidad;

namespace WpfCliente.Modelo
{
    public static class Conexion
    {
        public static bool CerrarConexionesServiciosSala()
        {
            try
            {
                if (Singleton.Instance.ServicioChatCliente != null)
                {
                    Singleton.Instance.ServicioChatCliente.Close();
                    Singleton.Instance.ServicioChatCliente = null;
                }
                if (Singleton.Instance.ServicioSalaJugadorCliente != null)
                {
                    Singleton.Instance.ServicioSalaJugadorCliente.Close();
                    Singleton.Instance.ServicioSalaJugadorCliente = null;
                }
            }
            catch (Exception excepcion)
            {
                //TODO Manejar el error
                return false;
            }
            return true;
        }
    }
}
