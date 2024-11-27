using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using DAOLibreria.Excepciones;

namespace DAOLibreria.DAO
{
    public static class UsuarioDAO
    {
        private const string PALABRA_RESERVADA_GUEST = "guest";
        /// <summary>
        /// Registra un nuevo usuario en el sistema junto con su cuenta asociada y estadísticas iniciales.
        /// Asegura la coherencia entre el gamertag del usuario y el de la cuenta.
        /// </summary>
        /// <param name="_usuario">El objeto Usuario que contiene la información del usuario, incluido el gamertag y foto de perfil.</param>
        /// <param name="_usuarioCuenta">El objeto UsuarioCuenta que contiene detalles de la cuenta del usuario como el gamertag, el hash de la contraseña y el correo electrónico.</param>
        /// <exception cref="VerificarNombreUnico(string)"></exception>
        /// <returns></returns>
        public static bool RegistrarNuevoUsuario(Usuario _usuario, UsuarioCuenta _usuarioCuenta)
        {
            bool resultado = false;
            if (_usuario == null || _usuarioCuenta == null)
            {
                return resultado;
            }
            VerificarNombreUnico(_usuario.gamertag);
            VerificarNombreUnico(_usuarioCuenta.gamertag);
            
            if (_usuario.gamertag != _usuarioCuenta.gamertag)
            {
                return resultado;
            }
            if (_usuario.gamertag.ToLower().Contains(PALABRA_RESERVADA_GUEST))
            {
                return resultado;
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var usuarioCuenta = new UsuarioCuenta
                            {
                                gamertag = _usuarioCuenta.gamertag,
                                hashContrasenia = _usuarioCuenta.hashContrasenia.ToUpper(),
                                correo = _usuarioCuenta.correo
                            };
                            context.UsuarioCuenta.Add(usuarioCuenta);
                            context.SaveChanges();
                            var usuario = new Usuario
                            {
                                gamertag = _usuario.gamertag,
                                fotoPerfil = _usuario.fotoPerfil,
                                ultimaConexion = null,
                                idUsuarioCuenta = usuarioCuenta.idUsuarioCuenta
                            };
                            context.Usuario.Add(usuario);
                            context.SaveChanges();
                            var estadisticas = new Estadisticas
                            {
                                idUsuario = usuario.idUsuario
                            };
                            context.Estadisticas.Add(estadisticas);
                            context.SaveChanges();
                            transaction.Commit();
                            resultado = true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            resultado = false;
                        }
                    }

                    return resultado;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuarioEditado"></param>
        /// <returns></returns>
        public static bool EditarUsuario(UsuarioPerfilDTO usuarioEditado)
        {
            bool resultado = false;
            if (usuarioEditado == null
                || usuarioEditado.IdUsuario <= 0
                || usuarioEditado.NombreUsuario == null)
            {
                return resultado;
            }
            if (usuarioEditado.NombreUsuario.ToLower().Contains(PALABRA_RESERVADA_GUEST))
            {
                return resultado;
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var usuario = context.Usuario.Single(b => b.gamertag == usuarioEditado.NombreUsuario);
                            var usuarioCuenta = context.UsuarioCuenta.Single(b => b.gamertag == usuarioEditado.NombreUsuario);
                            if (usuario.idUsuario != usuarioEditado.IdUsuario)
                            {
                                return false;
                            }
                            if (usuarioEditado.Correo != null)
                            {
                                usuario.UsuarioCuenta.correo = usuarioEditado.Correo;
                                resultado = true;
                            }
                            if (usuarioEditado.FotoPerfil != null)
                            {
                                usuario.fotoPerfil = usuarioEditado.FotoPerfil;
                                resultado = true;
                            }
                            if (usuarioEditado.HashContrasenia != null)
                            {
                                usuario.UsuarioCuenta.hashContrasenia = usuarioEditado.HashContrasenia;
                                resultado = true;
                            }
                            context.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }

                    return resultado;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gamertag"></param>
        /// <param name="contrasenia"></param>
        /// <returns></returns>
        public static UsuarioPerfilDTO ValidarCredenciales(string gamertag, string contrasenia)
        {
            UsuarioCuenta datosUsuarioCuenta = null;
            Usuario usuario = null;
            UsuarioPerfilDTO resultado = null;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    datosUsuarioCuenta = context.UsuarioCuenta
                                                 .Include("Usuario")
                                                 .SingleOrDefault(cuenta => cuenta.gamertag == gamertag);
                    if (datosUsuarioCuenta != null)
                    {
                        var contraseniaCuenta = datosUsuarioCuenta.hashContrasenia.ToUpper();

                        if (contrasenia.Equals(contraseniaCuenta, StringComparison.OrdinalIgnoreCase))
                        {
                            usuario = datosUsuarioCuenta.Usuario.FirstOrDefault();
                            resultado = new UsuarioPerfilDTO(usuario, datosUsuarioCuenta);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ValidarCredenciales: {ex.Message}");
            }

            return resultado;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gamertag"></param>
        /// <returns></returns>
        public static Usuario ObtenerUsuarioPorNombre(string gamertag)
        {
            Usuario usuario = null;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    usuario = context.Usuario
                                   .SingleOrDefault(userioFila => userioFila.gamertag == gamertag);
                }
            }

            catch (Exception)
            {
            }
            return usuario;
        }

        public static Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            Usuario usuario = null;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    usuario = context.Usuario
                                     .SingleOrDefault(userFila => userFila.idUsuario == idUsuario);
                }
            }
            catch (Exception)
            {
            }
            return usuario;
        }

        /// <summary>
        /// Obtiene el ID de un usuario por su gamertag.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <returns>El ID del usuario, o 0 si no se encuentra.</returns>
        public static int ObtenerIdPorNombre(string gamertag)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Usuario
                                         .SingleOrDefault(usuarioFila => usuarioFila.gamertag == gamertag);
                    if (usuario != null)
                    {
                        return usuario.idUsuario; 
                    }
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nombres"></param>
        /// <returns></returns>
        public static List<Usuario> ObtenerListaUsuariosPorNombres(List<string> nombres)
        {
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (var context = new DescribeloEntities())
                {
                    usuarios = context.Usuario
                        .Where(usuarioFila => nombres.Contains(usuarioFila.gamertag))
                        .ToList();
                }
            }
            catch (Exception )
            {
            }
            return usuarios;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gamertag"></param>
        /// <returns></returns>
        /// <exception cref="GamertagDuplicadoException"></exception>
        public static bool VerificarNombreUnico(string gamertag)
        {
            bool resultado = true;
            Usuario usuario = null;
            UsuarioCuenta usuarioCuenta = null;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    usuario = context.Usuario
                                   .SingleOrDefault(userioFila => userioFila.gamertag == gamertag);
                    usuarioCuenta = context.UsuarioCuenta
                                   .SingleOrDefault(userioFila => userioFila.gamertag == gamertag);
                }
            }

            catch (Exception)
            {
            }

            if (usuario != null || usuarioCuenta != null)
            {
                throw new GamertagDuplicadoException($"El gamertag '{gamertag}' ya está en uso.");
            }
            return resultado;
        }

        /// <summary>
        /// Verifica si existe únicamente un usuario con el gamertag y el correo especificados.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <param name="correo">El correo electrónico del usuario.</param>
        /// <returns>True si existe exactamente un usuario con ese gamertag y correo, de lo contrario, false.</returns>
        public static bool ExisteUnicoUsuarioConGamertagYCorreo(string gamertag, string correo)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var cantidadUsuarios = context.UsuarioCuenta
                        .Where(u => u.gamertag == gamertag && u.correo == correo)
                        .Count();

                    return (cantidadUsuarios == 1);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Edita la contraseña de un usuario especificado por su gamertag.
        /// </summary>
        /// <param name="gamertag">El gamertag del usuario.</param>
        /// <param name="nuevoHashContrasenia">El nuevo hash de la contraseña.</param>
        /// <returns>True si la contraseña se actualizó correctamente, de lo contrario, false.</returns>
        public static bool EditarContraseniaPorGamertag(string gamertag, string nuevoHashContrasenia)
        {
            bool resultado = false;
            if (string.IsNullOrEmpty(gamertag) || string.IsNullOrEmpty(nuevoHashContrasenia))
            {
                return resultado;
            }

            try
            {
                using (var context = new DescribeloEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var usuarioCuenta = context.UsuarioCuenta.SingleOrDefault(cuenta => cuenta.gamertag == gamertag);
                            if (usuarioCuenta == null)
                            {
                                return false; 
                            }

                            usuarioCuenta.hashContrasenia = nuevoHashContrasenia.ToUpper();
                            context.SaveChanges();

                            transaction.Commit();
                            resultado = true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return resultado;
        }

        public static bool ColocarUltimaConexion(int idUsuario)
        {
            bool resultado = false;
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var usuario = context.Usuario.FirstOrDefault(b => b.idUsuario == idUsuario);
                    if (usuario != null)
                    {
                        usuario.ultimaConexion = DateTime.Now;
                        context.SaveChanges(); 
                        resultado = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return resultado;
        }
    }
}
