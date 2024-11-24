using DAOLibreria.ModeloBD;
using System;
using System.Linq;
using DAOLibreria.Excepciones;

namespace DAOLibreria.DAO
{
    public class VetoDAO
    {
        private const int ID_INVALIDO = 0;
        public static bool ExisteTablaVetoPorIdCuenta(int idUsuarioCuenta)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    return context.Veto.Any(veto => veto.idUsuarioCuenta == idUsuarioCuenta);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente)
        {
            if (idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            try
            {
                using (var context = new DescribeloEntities())
                {

                    var nuevoVeto = new Veto
                    {
                        fechaInicio = DateTime.Now,
                        fechaFin = fechaFin,
                        esPermanente = esPermanente,
                        idUsuarioCuenta = idUsuarioCuenta
                    };

                    context.Veto.Add(nuevoVeto);

                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Verifica si hay algún veto activo o permanente asociado a una cuenta de usuario específica.
        /// </summary>
        /// <param name="idUsuarioCuenta">El identificador de la cuenta de usuario a verificar.</param>
        /// <returns>
        /// True si se produce un error inesperado durante la verificación.
        /// False si no hay vetos activos o si todos los vetos han expirado y no son permanentes.
        /// </returns>
        /// <exception cref="VetoPermanenteExcepcion">
        /// Lanza una excepción si se encuentra un veto permanente asociado a la cuenta.
        /// </exception>
        /// <exception cref="VetoEnProgresoExcepcion">
        /// Lanza una excepción si se encuentra un veto temporal que todavía está en progreso.
        /// </exception>
        /// /// <remarks>
        /// Este método consulta la base de datos para recuperar todos los vetos asociados con el identificador de cuenta dado.
        /// Basado en la evaluación de estos vetos, se pueden lanzar excepciones específicas para indicar la presencia de restricciones activas.
        /// La lógica de manejo de excepciones está diseñada para separar claramente los diferentes tipos de restricciones (permanentes y temporales).
        /// </remarks>
        public static bool VerificarVetoPorIdCuenta(int idUsuarioCuenta)
        {
            try
            {
                using (var context = new DescribeloEntities())
                {
                    var vetos = context.Veto
                                        .Where(v => v.idUsuarioCuenta == idUsuarioCuenta)
                                        .ToList();

                    if (!vetos.Any())
                    {
                        return false;
                    }

                    if (vetos.Any(v => v.esPermanente))
                    {
                        throw new VetoPermanenteExcepcion();
                    }

                    if (vetos.Any(v => v.fechaFin.HasValue && v.fechaFin > DateTime.Now))
                    {
                        throw new VetoEnProgresoExcepcion();
                    }
                    return false;
                }
            }
            catch (VetoPermanenteExcepcion)
            {
                throw new VetoPermanenteExcepcion();
            }
            catch (VetoEnProgresoExcepcion)
            {
                throw new VetoEnProgresoExcepcion();
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}

