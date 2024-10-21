using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using UtilidadesLibreria;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioRegistro
    {

        public bool RegistrarUsuario(Modelo.Usuario _usuario)
        {
            bool resultado = false;
            try
            {

                if (EsSha256Valido(_usuario.ContraseniaHASH))
                {
                    var usuarioCuenta = new UsuarioCuenta
                    {
                        gamertag = _usuario.Nombre,
                        hashContrasenia = _usuario.ContraseniaHASH.ToString(),
                        correo = _usuario.Correo
                    };
                    var usuario = new Usuario
                    {
                        gamertag = _usuario.Nombre,
                        fotoPerfil = Utilidad.StreamABytes(_usuario.FotoUsuario),
                    };
                    resultado = DAOLibreria.DAO.UsuarioDAO.RegistrarNuevoUsuario(usuario, usuarioCuenta);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
                Console.WriteLine(excepcion);
                Console.WriteLine(excepcion.StackTrace);
            }

            return resultado;
        }

        public bool EsSha256Valido(string hash)
        {
            return hash.Length == 64 && Regex.IsMatch(hash, @"\A\b[0-9a-fA-F]+\b\Z");
        }
    }
}
