using System;
using System.Collections.Generic;
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
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
                Console.WriteLine(excepcion);
                Console.WriteLine(excepcion.StackTrace);
            }

            return resultado;
        }

    }
}
