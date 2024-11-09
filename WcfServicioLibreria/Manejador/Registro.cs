using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo.Excepciones;
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
            }catch (GamertagDuplicadoException)
            {
                BaseDatosFalla excepcion = new BaseDatosFalla()
                {
                    EsGamertagDuplicado = true
                };
                throw new FaultException<BaseDatosFalla>(excepcion, new FaultReason("Gamertag en uso"));
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
                Console.WriteLine(excepcion);
                Console.WriteLine(excepcion.StackTrace);
            }

            return resultado;
        }
        /// <summary>
        /// Metodo para evaluar si cumple con nuestro criterio de encriptacion por SHA256
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>Si es valido retorna true, en caso contraio false</returns>
        public bool EsSha256Valido(string hash)
        {
            if (hash.Length != 64) return false;

            return hash.All(c => "0123456789abcdefABCDEF".Contains(c));
        }
    }
}
