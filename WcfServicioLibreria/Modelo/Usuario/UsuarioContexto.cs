using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;
using WcfServicioLibreria.Evento;
using DAOLibreria.ModeloBD;
namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-7.0</ref>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-idisposable</ref>
    [DataContract]
    public abstract class UsuarioContexto
    {
        protected int idUsuario;
        protected String nombre;
        private bool desechado = false;
        public IUsuarioSesionCallback UsuarioSesionCallBack { get; set; }
        public IAmistadCallBack AmistadSesionCallBack { get; set; }
        public EventHandler CerrandoEvento { get; set; }
        public EventHandler CerradoEvento { get; set; }
        public EventHandler FalloEvento { get; set; }
        public EventHandler DesconexionManejadorEvento;
        public EventHandler ActulizarAmigoManejadorEvento;
        /// <summary>
        /// Avisa a todos los sucriptores que se esta desconectando
        /// </summary>
        public void EnDesconexion()
        {
            DesconexionManejadorEvento?.Invoke(idUsuario, new UsuarioDesconectadoEventArgs(nombre, DateTime.Now, idUsuario));
        }
        //FIXME
        public void AmigoDesconectado(object sender, EventArgs e)
        {
            var desconectadoArgs = e as UsuarioDesconectadoEventArgs;
            if (desconectadoArgs != null)
            {
                Amigo amigo = new Amigo
                {
                    Nombre = desconectadoArgs.NombreUsuario,
                    Estado = desconectadoArgs.EstadoAmigo
                };
                AmistadSesionCallBack.CambiarEstadoAmigo(amigo);
            }
        }

        public void EnviarAmigoActulizadoCallback(Amigo amigo)
        {
            AmistadSesionCallBack.CambiarEstadoAmigo(amigo);

        }

        /// <summary>
        /// Libera recursos que de otra manera no serian liberados
        /// </summary>
        public virtual void Desechar()
        {
            if (desechado)
                return;
            DesuscribirseDeEventos();
            UsuarioSesionCallBack = null;
            desechado = true;

        }
        /// <summary>
        /// Elimina las suscripciones los eventos de ICommunicationObject de la sesion de IUsuarioSesionCallback
        /// </summary>
        private void DesuscribirseDeEventos()
        {
            if (UsuarioSesionCallBack is ICommunicationObject comunicacionObjecto)
            {
                if (CerrandoEvento != null)
                {
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
