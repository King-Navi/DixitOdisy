using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class EstadisticaUsuario
    {
        public Estadistica Estadistica { get; set; }

        public EstadisticaUsuario(int idUsuario)
        {
            SolicitarEstadisiticas(idUsuario);
        }

        public void SolicitarEstadisiticas(int idUsuario)
        {
            var manejadorServicio = new ServicioManejador<ServicioEstadisticasClient>();
            var resutlado = manejadorServicio.EjecutarServicio(servicio =>
                servicio.ObtenerEstadisitca(idUsuario
           ));
            if (resutlado != null)
            {
                Estadistica = resutlado;
            }
        }   
        public async void SolicitarEstadisiticasAsync(int idUsuario)
        {
            var manejadorServicio = new ServicioManejador<ServicioEstadisticasClient>();
            var resutlado = await manejadorServicio.EjecutarServicioAsync(servicio =>
                servicio.ObtenerEstadisitcaAsync(idUsuario
           ));
            if (resutlado != null)
            {
                Estadistica = resutlado;
            }
        }


    }
}
