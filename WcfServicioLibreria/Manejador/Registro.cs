using System;
using System.Linq;
using System.ServiceModel;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioRegistro
    {
        private const string NOMBRE_EN_USO = "Gamertag en uso";
        public bool RegistrarUsuario(Modelo.Usuario usuarioNuevo)
        {
            bool resultado = false;
            try
            {
                if (EsSha256Valido(usuarioNuevo.ContraseniaHASH) && Utilidad.ValidarPropiedades(usuarioNuevo))
                {
                    var usuarioCuenta = new UsuarioCuenta
                    {
                        gamertag = usuarioNuevo.Nombre,
                        hashContrasenia = usuarioNuevo.ContraseniaHASH.ToString(),
                        correo = usuarioNuevo.Correo
                    };
                    var usuarioModeloBaseDatos = new Usuario
                    {
                        gamertag = usuarioNuevo.Nombre,
                        fotoPerfil = Utilidad.StreamABytes(usuarioNuevo.FotoUsuario),
                    };
                    resultado = usuarioDAO.RegistrarNuevoUsuario(usuarioModeloBaseDatos, usuarioCuenta);
                }
            }
            catch (GamertagDuplicadoException)
            {
                BaseDatosFalla excepcion = new BaseDatosFalla()
                {
                    EsGamertagDuplicado = true
                };
                throw new FaultException<BaseDatosFalla>(excepcion, new FaultReason(NOMBRE_EN_USO));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return resultado;
        }

        public bool EsSha256Valido(string hash)
        {
            if (hash.Length != 64) return false;

            return hash.All(c => "0123456789abcdefABCDEF".Contains(c));
        }
    }
}
