using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using System;
using System.IO;
using System.Security.Principal;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuario
    {
        /// <summary>
        /// Metodo que desconecta a un jugador y lo elimna de memoria. 
        ///  Despues de ejecutar este metodo el usario ya no existe en memoria acausa del dispose()
        /// </summary>
        /// <param name="idJugador"></param>
        public void DesconectarUsuario(int idJugador)
        {
            try
            {
                if (jugadoresConectadosDiccionario.ContainsKey(idJugador))
                {
                    jugadoresConectadosDiccionario.TryRemove(idJugador, out UsuarioContexto usuario);
                    lock (usuario)
                    {
                        if (usuario != null)
                        {
                            usuario.EnDesconexion();
                            usuario.Desechar();
                        }
                    }

                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO : Manejar el error
            }
        }

        public bool EditarUsuario(Modelo.Usuario usuarioEditado)
        {
            bool resultado = false;
            if (usuarioEditado == null
                || usuarioEditado.IdUsuario <= 0
                || usuarioEditado.Nombre == null)
            {
                return resultado;
            }
            if (usuarioEditado.ContraseniaHASH != null)
            {
                if (!EsSha256Valido(usuarioEditado.ContraseniaHASH))
                    return resultado;
            }
            try
            {
                DAOLibreria.ModeloBD.UsuarioPerfilDTO usuarioPerfilDTO = new UsuarioPerfilDTO()
                {
                    IdUsuario = usuarioEditado.IdUsuario,
                    NombreUsuario = usuarioEditado.Nombre,
                    Correo = usuarioEditado.Correo ?? null,
                    FotoPerfil = usuarioEditado.FotoUsuario != null ? Utilidad.StreamABytes(usuarioEditado.FotoUsuario) : null,
                    HashContrasenia = usuarioEditado.ContraseniaHASH ?? null,
                };
                bool consulta = DAOLibreria.DAO.UsuarioDAO.EditarUsuario(usuarioPerfilDTO);
                if (consulta)
                {
                    //TOOD: Si en memoria cargamso anteriormente valores del usuario cambiarlos por los nuevo
                    //en teoria bo deberiamos cargar esos datos.
                    //jugadoresConectadosDiccionario.TryGetValue(usuarioEditado.IdUsuario, out UsuarioContexto usuario );

                    //if (usuarioEditado.Correo != null)
                    //    ((Modelo.Usuario)usuario).Correo = usuarioEditado.Correo;

                    //if (usuarioEditado.FotoPerfil != null)
                    //    ((Modelo.Usuario)usuario).fo = usuarioEditado.FotoPerfil;

                    //if (usuarioEditado.HashContrasenia != null)
                    //    ((Modelo.Usuario)usuario).ContraseniaHASH = usuarioEditado.HashContrasenia;

                    resultado = true;
                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO:manejar error
            }
            return resultado;
        }

        public bool Ping()
        {
            return true;
        }

        public WcfServicioLibreria.Modelo.Usuario ValidarCredenciales(string gamertag, string contrasenia)
        {
            WcfServicioLibreria.Modelo.Usuario usuario = null;
            try
            {
                DAOLibreria.ModeloBD.UsuarioPerfilDTO usuarioConsulta = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertag, contrasenia);
                if (usuarioConsulta != null)
                {
                    usuario = new WcfServicioLibreria.Modelo.Usuario();
                    if (usuarioConsulta.FotoPerfil != null && usuarioConsulta.FotoPerfil.Length > 0)
                    {
                        usuario.FotoUsuario = new MemoryStream(usuarioConsulta.FotoPerfil, writable: false);
                    }

                    usuario.Nombre = usuarioConsulta.NombreUsuario;
                    usuario.Correo =usuarioConsulta.Correo; 
                    usuario.ContraseniaHASH = usuarioConsulta.HashContrasenia;
                    usuario.IdUsuario = usuarioConsulta.IdUsuario;
                }
            }
            catch (Exception ex)
            {
                //TODO manejar la excepcion
            }

            return usuario;
        }


        public bool YaIniciadoSesion(string nombreUsuario)
        {
            //TODO: Hacer una consulta para obtener 
            //ObtenerIdPorNombre();
            //TODO: No dejar el 1 y cambiar por el resultado de la consulta
            return jugadoresConectadosDiccionario.ContainsKey(1);
        }

    }
}
