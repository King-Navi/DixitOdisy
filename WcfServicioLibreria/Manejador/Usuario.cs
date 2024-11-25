using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuario
    {
        /// <summary>
        /// Metodo que desconecta a un jugador y lo elimna de memoria. 
        ///  Despues de ejecutar este metodo el usario ya no existe en memoria acausa del desechar()
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
                    UsuarioDAO.ColocarUltimaConexion(idJugador);
                }
            }
            catch (Exception)
            {
            }
        }

        public bool EditarContraseniaUsuario(string gamertag, string nuevaContrasenia)
        {
            bool resultado = false;

            if (string.IsNullOrEmpty(gamertag) || string.IsNullOrEmpty(nuevaContrasenia))
            {
                return resultado;
            }

            if (!EsSha256Valido(nuevaContrasenia))
            {
                return resultado;
            }

            try
            {
                resultado = UsuarioDAO.EditarContraseniaPorGamertag(gamertag, nuevaContrasenia);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Error al editar contraseña para el usuario {gamertag}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al editar contraseña para el usuario {gamertag}: {ex.Message}");
            }

            return resultado;
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
                resultado = true;

            }
            catch (Exception) 
            {
            }
            return resultado;
        }

        public bool Ping()
        {
            return true;
        }

        public async Task<bool> PingBDAsync()
        {
            return await DAOLibreria.ModeloBD.Conexion.VerificarConexionAsync();
        }

        public WcfServicioLibreria.Modelo.Usuario ValidarCredenciales(string gamertag, string contrasenia)
        {
            WcfServicioLibreria.Modelo.Usuario usuario = null;
            try
            {
                DAOLibreria.ModeloBD.UsuarioPerfilDTO usuarioConsulta = DAOLibreria.DAO.UsuarioDAO.ValidarCredenciales(gamertag, contrasenia);
                if (usuarioConsulta != null)
                {
                    DAOLibreria.DAO.VetoDAO.VerificarVetoPorIdCuenta(usuarioConsulta.IdUsuarioCuenta);
                    usuario = new WcfServicioLibreria.Modelo.Usuario();
                    if (usuarioConsulta.FotoPerfil != null && usuarioConsulta.FotoPerfil.Length > 0)
                    {
                        usuario.FotoUsuario = new MemoryStream(usuarioConsulta.FotoPerfil, writable: false);
                    }
                    
                    usuario.Nombre = usuarioConsulta.NombreUsuario;
                    usuario.Correo = usuarioConsulta.Correo;
                    usuario.ContraseniaHASH = usuarioConsulta.HashContrasenia;
                    usuario.IdUsuario = usuarioConsulta.IdUsuario;
                }

            }
            catch(VetoEnProgresoExcepcion )
            {
                VetoFalla excepcion = new VetoFalla()
                {
                    EnProgreso = true
                };
                throw new FaultException<VetoFalla>(excepcion, new FaultReason("Veto en progreso"));
            }
            catch (VetoPermanenteExcepcion)
            {
                VetoFalla excepcion = new VetoFalla()
                {
                    EsPermanete = true
                };
                throw new FaultException<VetoFalla>(excepcion, new FaultReason("Veto en permanete, y no vuelvas!!!"));
            }
            catch (Exception)
            {
            }

            return usuario;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <returns>retorna true si contiene un usuario con el nombre, en caso contrario retorna false</returns>
        public bool YaIniciadoSesion(string nombreUsuario)
        {
            DAOLibreria.ModeloBD.Usuario usuario = DAOLibreria.DAO.UsuarioDAO.ObtenerUsuarioPorNombre(nombreUsuario);
            if (usuario == null)
            {
                return false;
            }
            return jugadoresConectadosDiccionario.ContainsKey(usuario.idUsuario);
        }

        public void JugadoresConectado()
        {
            foreach (UsuarioContexto jugador in jugadoresConectadosDiccionario.Values)
            {
                try
                {
                    Modelo.Usuario usuarioActual = jugador as Modelo.Usuario;
                    Console.WriteLine(usuarioActual.Nombre);

                }
                catch (Exception)
                {
                }
            }
        }

        

    }
}
