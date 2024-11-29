using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class EstadisticaUsuario
    {
        public Estadistica Estadistica { get; set; }

        public EstadisticaUsuario(int idUsuario)
        {
            SolicitarEstadisticas(idUsuario);
        }

        public void SolicitarEstadisticas(int idUsuario)
        {
            var manejadorServicio = new ServicioManejador<ServicioEstadisticasClient>();
            var resultado = manejadorServicio.EjecutarServicio(servicio =>
                servicio.ObtenerEstadisticas(idUsuario
           ));
            if (resultado != null)
            {
                Estadistica = resultado;
            }
        }   
        public async Task SolicitarEstadisticasAsync(int idUsuario)
        {
            var manejadorServicio = new ServicioManejador<ServicioEstadisticasClient>();
            var resultado = await manejadorServicio.EjecutarServicioAsync(servicio =>
                servicio.ObtenerEstadisticasAsync(idUsuario
           ));
            if (resultado != null)
            {
                Estadistica = resultado;
            }
        }


    }
}
