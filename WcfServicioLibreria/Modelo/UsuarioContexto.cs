using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public abstract class UsuarioContexto : IDisposable
    {
        public IUsuarioSesionCallback UsuarioSesionCallBack { get; set; }
        public EventHandler CerrandoEvento { get; set; }
        public EventHandler CerradoEvento { get; set; }
        public EventHandler FalloEvento { get; set; }

        public void Dispose()
        {
            // Llamar al método Dispose en la clase base si es necesario
            Dispose(true);

            // Suprimir la llamada al finalizador
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool desechar)
        {
            if (desechar)
            {
                // Liberar recursos gestionados
                DesuscribirseDeEventos();
                UsuarioSesionCallBack = null;
                // Otros recursos gestionados
            }
            // Liberar recursos no gestionados si los hay
        }
        private void DesuscribirseDeEventos()
        {
            if (UsuarioSesionCallBack is ICommunicationObject comunicacionObjecto)
            {
                if (CerrandoEvento != null)
                {
                    comunicacionObjecto.Closing -= CerrandoEvento;
                    CerrandoEvento = null;
                }

                if (CerradoEvento != null)
                {
                    comunicacionObjecto.Closed -= CerradoEvento;
                    CerradoEvento = null;
                }

                if (FalloEvento != null)
                {
                    comunicacionObjecto.Faulted -= FalloEvento;
                    FalloEvento = null;
                }
            }
        }

    }
}
