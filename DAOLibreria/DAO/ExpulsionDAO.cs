using DAOLibreria.ModeloBD;
using System;
using System.Linq;

namespace DAOLibreria.DAO
{
    public class ExpulsionDAO
    {
        private const int ID_INVALIDO = 0;
        public static bool TieneMasDeDiezExpulsionesSinPenalizar(int idUsuarioCuenta)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    int conteo = context.Expulsion
                                        .Count(expulsion => expulsion.idUsuarioCuenta == idUsuarioCuenta && !expulsion.fuePenalizado);

                    return conteo >= 10; 
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool CambiarExpulsionesAFueronPenalizadas(int idUsuarioCuenta)
        {
            if (idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var expulsiones = context.Expulsion
                                              .Where(expulsion => expulsion.idUsuarioCuenta == idUsuarioCuenta && !expulsion.fuePenalizado)
                                              .ToList();

                    if (!expulsiones.Any())
                    {
                        return true;
                    }
                    foreach (var expulsion in expulsiones)
                    {
                        expulsion.fuePenalizado = true;
                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception )
            {
            }
            return false;
        }

        public static bool CrearRegistroExpulsion(int idUsuarioCuenta, string motivo, bool esHacker)
        {
            if (idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var nuevaExpulsion = new Expulsion
                    {
                        motivo = motivo,
                        fuePenalizado = false,
                        esHacker = esHacker,
                        idUsuarioCuenta = idUsuarioCuenta
                    };
                    context.Expulsion.Add(nuevaExpulsion);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
