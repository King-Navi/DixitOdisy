using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manegador
{
    public partial class Managador : IServicioRegistro
    {

        public void RegistrarNuevoUsuario(string nombreUsuario, string contrasenia) 
        {
            //TODO Valdidar contrasenia
            ValidarContrasenia(contrasenia);


            Usuario usuario = new Usuario() {
                Nombre = nombreUsuario,
                ContraseniaHASH = SHA256.Create(contrasenia)
            };

            //TODO Enviar a la base de datos
            return ;
        }

        private void ValidarContrasenia(string contrasenia)
        {
            //TODO Validar politica de contraseña
        }
    }
}
