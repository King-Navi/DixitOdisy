using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    [ServiceBehavior(InstanceContextMode =InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ManejadorPrincipal : IServicioChat
    {
        /// <summary>
        /// Alamacena los usuarios conectados
        /// </summary>
        private List<ChatUsuario> usuariosConectados = new List<ChatUsuario>();
        /// <summary>
        /// Diccionario que guarda los mensajes
        /// </summary>
        private Dictionary<string, List<ChatMensaje>> mensajeEntrante = new Dictionary<string, List<ChatMensaje>>();

        public List<ChatUsuario> UsuariosConectados { get => usuariosConectados; set => usuariosConectados = value; }

        public void DesconectarUsuario(ChatUsuario usuario)
        {
            this.usuariosConectados.RemoveAll(usuarioActual => usuarioActual.NombreUsuario == usuario.NombreUsuario);
            //this.UsuariosConectados.Remove(usuario);
        }
        /// <summary>
        /// Agrega un usuario al chat
        /// </summary>
        /// <param name="usuarioNuevo"></param>
        /// <returns></returns>

        public ChatUsuario ConectarUsuario(string usuario)
        {
            ChatUsuario usuarioNuevo = new ChatUsuario() { NombreUsuario = usuario };
            var usuarioExistente = usuariosConectados.Any(e => e.NombreUsuario == usuarioNuevo.NombreUsuario);
            if (!usuarioExistente)
            {
                this.usuariosConectados.Add(usuarioNuevo);
                mensajeEntrante.Add(usuarioNuevo.NombreUsuario, new List<ChatMensaje>()
                {
                    new ChatMensaje() {Usuario = usuarioNuevo, HoraFecha= DateTime.Now, Mensaje="Bienvenido al WPF chat"}
                });
                Console.WriteLine("\nNuevo usuario conectado: " + usuarioNuevo.NombreUsuario);
                return usuarioNuevo;
            }
            else
            {
                return null;
            }
        }

        public List<ChatUsuario> ObtenerUsuariosConectados()
        {
            return this.usuariosConectados;
        }

        public void EnviarMensaje(ChatMensaje nuevoMensaje)
        {
            foreach (var usuarioActual in this.usuariosConectados)
            {
                if (nuevoMensaje.Usuario.NombreUsuario != usuarioActual.NombreUsuario)
                {
                    Console.WriteLine("El mensaje se agrego a la cola a: " + usuarioActual.NombreUsuario);
                    if (mensajeEntrante.ContainsKey(usuarioActual.NombreUsuario))
                    {
                        mensajeEntrante[usuarioActual.NombreUsuario].Add(nuevoMensaje);
                    }
                }
            }
        }

        public List<ChatMensaje> GetMensajeNuevo(ChatUsuario usuario)
        {
            if (!String.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                Console.WriteLine("usuario.Nombre es vacio");
                return new List<ChatMensaje>(); // Devuelve una lista vacía en lugar de null
            }
            try
            {
                if (mensajeEntrante.TryGetValue(usuario.NombreUsuario, out var nuevosMensajes))
                {
                    if (nuevosMensajes.Count > 0)
                    {
                        var mensajesParaDevolver = new List<ChatMensaje>(nuevosMensajes);
                        nuevosMensajes.Clear(); // Limpia los mensajes después de devolverlos
                        return mensajesParaDevolver;
                    }
                }
                return new List<ChatMensaje>(); // Devuelve una lista vacía si no hay mensajes o la clave no se encuentra
            }
            catch (KeyNotFoundException error)
            {
                //TODO manejar el error
                Console.WriteLine("Nombre no encontrado: " + error.Message);
                return null;
            }
        }
    }
}
