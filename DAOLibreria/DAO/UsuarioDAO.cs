using DAOLibreria.ModeloBD;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using UtilidadesLibreria;

namespace DAOLibreria.DAO
{
    public static class UsuarioDAO
    {
        //public static Dictionary<string, object> CrearUsuario(string usuario, string contrasenia)
        //{
        //    Dictionary<string, object> resultado = new Dictionary<string, object>();

        //    DAOLibreria.ModeloBD. usuarioNuevo = new DAOLibreria.ModeloBD.usuario()
        //    {
        //        nombre = usuario,
        //        contraseniaHash = contrasenia
        //    };
        //    try
        //    {
        //        using (var context = new DescribeloTestEntities())
        //        using (var transaction = context.Database.BeginTransaction())
        //        {
        //            context.usuario.Add(usuarioNuevo);
        //            context.SaveChanges();
        //            transaction.Commit();
        //            resultado.Add(Llaves.LLAVE_ERROR, false);
        //            resultado.Add(Llaves.LLAVE_MENSAJE, "Usuario creado con éxito.");
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        //TODO Manejar excepciones de actualización de la base de datos
        //        resultado.Add(Llaves.LLAVE_ERROR, true);
        //        resultado.Add(Llaves.LLAVE_MENSAJE, "Error al crear el usuario. Verifique los datos." + ex);
        //    }
        //    catch (EntityException ex)
        //    {
        //        //TODO Manejar excepciones relacionadas con Entity Framework
        //        resultado.Add(Llaves.LLAVE_ERROR, true);
        //        resultado.Add(Llaves.LLAVE_MENSAJE, "Error de conexión con la base de datos." + ex);
        //    }

        //    return resultado;
        //}
    }
}
