using System;
using System.IO;
using System.Runtime.Serialization;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public abstract class UsuarioContexto : IObservador
    {
        protected int idUsuario;
        protected String nombre;
        private bool desechado = false;
        public IUsuarioSesionCallback UsuarioSesionCallback { get; set; }
        public EventHandler DesconexionEvento;
        public DesconectorEventoManejador DesconexionManejador { get; set; }

        protected UsuarioContexto() { }
        protected UsuarioContexto (int _idUsuario, string _nombre, IUsuarioSesionCallback _usuarioSesionCallback)
        {
            this.idUsuario = _idUsuario;
            this.nombre = _nombre;
            UsuarioSesionCallback = _usuarioSesionCallback;
        }

        public void EnDesconexion()
        {
            DesconexionEvento?.Invoke(this, new UsuarioDesconectadoEventArgs(nombre, DateTime.Now, idUsuario));
        }
        public void AmigoDesconectado(object sender, EventArgs e)
        {
            if (e is UsuarioDesconectadoEventArgs desconectadoArgs && sender is DAOLibreria.ModeloBD.Usuario usuarioDatos)
            {
                try
                {
                    Amigo amigo = new Amigo
                    {
                        Nombre = usuarioDatos.gamertag,
                        Estado = Enumerador.EstadoAmigo.Desconectado,
                        UltimaConexion = usuarioDatos.ultimaConexion.ToString(),
                        Foto = usuarioDatos.fotoPerfil
                    };

                    UsuarioSesionCallback?.CambiarEstadoAmigoCallback(amigo);
                }
                catch (Exception excecpion)
                {
                    ManejadorExcepciones.ManejarExcepcionError(excecpion);
                }
            }
        }

        public void EnviarAmigoActulizadoCallback(Amigo amigo)
        {
            try
            {
                Console.WriteLine($"{this.nombre} + {amigo.Nombre}");
                UsuarioSesionCallback.CambiarEstadoAmigoCallback(amigo);
            }
            catch (Exception excecpion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excecpion);
            }
        }

        public virtual void Desechar()
        {
            if (desechado)
                return;
            UsuarioSesionCallback = null;
            desechado = true;

        }

        public void DesconectarUsuario(string nombreJugador)
        {
            EnDesconexion();
        }
    }
}
