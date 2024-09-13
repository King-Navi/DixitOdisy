using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioRegistro
    {

        public void RegistrarUsuario(string usuario, string contrasenia)
        {
            //TODO Valdidar contrasenia
            ValidarContrasenia(contrasenia);


            Usuario usuarioNuevo = new Usuario()
            {

                Nombre = usuario,
                ContraseniaHASH = SHA256.Create(contrasenia)
            };

            //TODO Enviar a la base de datos
            return;
        }

        private void ValidarContrasenia(string contrasenia)
        {
            //TODO Validar politica de contraseña
        }
    }
}
