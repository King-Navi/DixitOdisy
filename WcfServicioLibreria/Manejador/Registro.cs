using System;
using System.Collections.Generic;
using DAOLibreria.DAO;
using UtilidadesLibreria;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioRegistro
    {

        public int RegistrarUsuario(string usuario, string contrasenia)
        {
            int resultado;
            Dictionary<string, object> consulta = 
                UsuarioDAO.CrearUsuario(usuario, contrasenia);
            if (!consulta.TryGetValue(Llaves.LLAVE_ERROR, out _))
            {
                Console.WriteLine(usuario + "fallo al registralo");
                consulta.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
                Console.WriteLine(mensaje);
                resultado = -1;
            }
            else 
            {
                Console.WriteLine(usuario + "Se registro");
                resultado = 1;

            }

            return resultado;
        }

    }
}
