using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using UtilidadesLibreria;

namespace DAOLibreria.DAO
{
    public static class UsuarioDAO
    {
        public static bool RegistrarNuevoUsuario(Usuario _usuario, UsuarioCuenta _usuarioCuenta)
        {
            bool resultado = false;
            if (_usuario.gamertag != _usuarioCuenta.gamertag)
            {
                return resultado;
            }
            using (var context = new DescribeloEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var usuarioCuenta = new UsuarioCuenta
                        {
                            gamertag = _usuarioCuenta.gamertag,
                            hashContrasenia = _usuarioCuenta.hashContrasenia,
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
                    catch (Exception excepcion)
                    {
                        transaction.Rollback();

                        //TODO: Manejar el error
                        Console.WriteLine(excepcion);
                        Console.WriteLine(excepcion.StackTrace);
                        throw;
                    }
                }

                return resultado;
            }
        }

        public static Usuario ValidarCredenciales(string gamertag, string contrasenia)
        {
            UsuarioCuenta datosUsuarioCuenta = null;
            Usuario usuario = null;

            try
            {
                using (var context = new DescribeloEntities())
                {
                    datosUsuarioCuenta = context.UsuarioCuenta.Include("Usuario").SingleOrDefault(cuenta => cuenta.gamertag == gamertag);
                }
            }

            catch (Exception ex)
            {
                //TODO manejar excepcion
            }

            if (datosUsuarioCuenta != null)
            {
                var contraseniaCuenta = datosUsuarioCuenta.hashContrasenia.ToUpper();

                if (contrasenia == contraseniaCuenta)
                {
                    usuario = GetUsuarioById(datosUsuarioCuenta.idUsuarioCuenta);
                }
            }

            return usuario;
        }

        public static Usuario GetUsuarioById(int id)
        {
            Usuario usuario = null;

            try
            {
                using (var context = new DescribeloEntities())
                {
                    usuario = context.Usuario
                                   .SingleOrDefault(user => user.idUsuario == id);
                }
            }

            catch (Exception ex)
            {
                //TODO manejar excepcion
            }
            return usuario;
        }
    }
}
