using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuario
    {

        private void DesconectarJugador(int idJugador)
        {
            try
            {
                if (jugadoresConetcadosDiccionario.ContainsKey(idJugador))
                {
                    jugadoresConetcadosDiccionario.TryRemove(idJugador, out UsuarioContexto usuario);
                    if (usuario != null)
                    {
                        usuario.Dispose();
                    }
                }
            }
            catch (CommunicationException ex)
            {
                   //TODO : Manejar el error
            }
        }
    }
}
