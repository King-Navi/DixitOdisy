using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente
{
    public sealed class Resgistro
    {

        


        public static void EnviarRegistro(String usuario, String contrasenia) 
        {
            SHA256 contraseniaSHA256 = SHA256.Create(contrasenia);

        }
    }
}
