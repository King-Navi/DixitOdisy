using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using System;
using System.Security.Principal;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

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

        public bool EditarUsuario(Modelo.Usuario usuario)
        {
            bool resultado = false;
            try
            {

                resultado = true;
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
            WcfServicioLibreria.Modelo.Usuario usuario = new Modelo.Usuario();

            try
            {
                DAOLibreria.ModeloBD.Usuario usuarioConsulta = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertag, contrasenia);
                usuario.FotoUsuario = null; //FIXME
                usuario.Nombre = usuarioConsulta.gamertag;
                usuario.Correo = null; //FIXME
                usuario.ContraseniaHASH = contrasenia;
                usuario.IdUsuario = usuarioConsulta.idUsuario;

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
