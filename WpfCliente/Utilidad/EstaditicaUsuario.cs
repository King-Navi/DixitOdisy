using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class EstaditicaUsuario
    {
        public static Estadistica Estadistica { get; set; }

        public EstaditicaUsuario() { }

        public EstaditicaUsuario(int idEstaidisca)
        {
            SolicitarEstadisiticas(idEstaidisca);
        }

        public void SolicitarEstadisiticas(int idEstadisca)
        {
            var manejadorServicio = new ServicioManejador<ServicioEstadisticasClient>();
            var resutlado = manejadorServicio.EjecutarServicio(proxy =>
                proxy.ObtenerEstadisitca(idEstadisca
           ));
            if (resutlado != null)
            {
                Estadistica = resutlado;
            }
        }


    }
}
