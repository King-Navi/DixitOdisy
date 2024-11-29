﻿using DAOLibreria.ModeloBD;
using System;
using System.Linq;
using DAOLibreria.Excepciones;
using DAOLibreria.Utilidades;
using System.Data.Entity.Infrastructure;

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
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
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
            return false;
        }

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
            catch (ArgumentNullException excepcion)
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

