using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using UtilidadesLibreria;

namespace DAOLibreria.DAO
{
    public static class UsuarioDAO
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema junto con su cuenta asociada y estadísticas iniciales.
        /// Asegura la coherencia entre el gamertag del usuario y el de la cuenta.
        /// </summary>
        /// <param name="_usuario">El objeto Usuario que contiene la información del usuario, incluido el gamertag y foto de perfil.</param>
        /// <param name="_usuarioCuenta">El objeto UsuarioCuenta que contiene detalles de la cuenta del usuario como el gamertag, el hash de la contraseña y el correo electrónico.</param>
        /// <returns>True si el registro fue exitoso; False si los gamertags no coinciden o si ocurre un error durante la operación de la base de datos.</returns>
        /// <exception cref="Exception">Lanza una excepción si ocurre un error durante la transacción de la base de datos.</exception>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Verifica que los gamertags de Usuario y UsuarioCuenta coincidan.
        /// 2. Si ocurre un error durante la transacción, la misma se revierte y el error se maneja lanzando una excepción.
        /// </remarks>
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
                            hashContrasenia =_usuarioCuenta.hashContrasenia,
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
                        Console.WriteLine( excepcion.StackTrace);
                        throw;
                    }
                }

                return resultado;
            }
        }
        public static bool EditarUsuario(Usuario _usuario, UsuarioCuenta _usuarioCuenta)
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
                        var usuario = context.Usuario.Single(b => b.idUsuario == _usuario.idUsuario);
                        //TODO: Cambiar las cosas que digas con unaay
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
    }
}
