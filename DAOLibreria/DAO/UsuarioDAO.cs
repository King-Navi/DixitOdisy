using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using DAOLibreria.Excepciones;
using System.Data.Entity.Infrastructure;
using DAOLibreria.Utilidades;
using System.Runtime.InteropServices;

namespace DAOLibreria.DAO
{
    public static class UsuarioDAO
    {
        private const string PALABRA_RESERVADA_GUEST = "guest";

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
                        catch (DbUpdateException excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        catch (ArgumentNullException excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        catch (Exception excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        return resultado;
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }

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
                        catch (DbUpdateException excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        catch (ArgumentNullException excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        catch (Exception excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                        return resultado;
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }


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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
        }
    

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

            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return usuario;
        }

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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return 0;
        }


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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return usuarios;
        }


        public static void VerificarNombreUnico(string gamertag)
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
                if (usuario != null || usuarioCuenta != null)
                {
                    throw new GamertagDuplicadoException($"El gamertag '{gamertag}' ya está en uso.");
                }
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

        }


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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (OverflowException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }


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
                        catch (Exception excepcion)
                        {
                            transaction.Rollback();
                            ManejadorExcepciones.ManejarErrorException(excepcion);
                        }
                    }
                }
            }
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
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
            catch (DbUpdateException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
        }
    }
}
