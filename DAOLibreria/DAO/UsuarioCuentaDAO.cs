using DAOLibreria.Interfaces;
using DAOLibreria.Utilidades;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DAOLibreria.ModeloBD;


namespace DAOLibreria.DAO
{
    public class UsuarioCuentaDAO : IUsuarioCuentaDAO
    {
        public int ObtenerIdUsuarioCuentaPorIdUsuario(int idUsuario)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var idUsuarioCuenta = context.Usuario
                        .Where(u => u.idUsuario == idUsuario)
                        .Select(u => u.idUsuarioCuenta)
                        .FirstOrDefault();

                    return idUsuarioCuenta <= 0 ? -2 : idUsuarioCuenta;
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
            return -1;

        }

        public bool EditarContraseniaPorGamertag(string gamertag, string nuevoHashContrasenia)
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

        public bool ExisteUnicoUsuarioConGamertagYCorreo(string gamertag, string correo)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var cantidadUsuarios = context.UsuarioCuenta
                        .Where(u => u.gamertag == gamertag && u.correo == correo)
                        .Count();

                    return (cantidadUsuarios >= 1);
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
    }
}
