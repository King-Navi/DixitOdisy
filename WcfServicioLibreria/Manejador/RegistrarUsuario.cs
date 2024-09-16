using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UtilidadesLibreria;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioRegistro
    {

        public int RegistrarUsuario(string usuario, string contrasenia)
        {
            int resultado;
            Dictionary<string, object> consulta = 
                DAOLibreria.DAO.UsuarioDAO.CrearUsuario(usuario, contrasenia);
            if (!(bool)consulta.TryGetValue(Llaves.LLAVE_ERROR, out object error))
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
