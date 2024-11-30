using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.Linq;
using System.ServiceModel;
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
                if (EsSha256Valido(usuarioNuevo.ContraseniaHASH) && ValidarUsuarioRegistro(usuarioNuevo))
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
            catch (GamertagDuplicadoException excepcionGamertag)
            {
                BaseDatosFalla excepcionBaseDatos = new BaseDatosFalla()
                {
                    EsGamertagDuplicado = true
                };
                ManejadorExcepciones.ManejarErrorException(excepcionGamertag);
                throw new FaultException<BaseDatosFalla>(excepcionBaseDatos, new FaultReason(NOMBRE_EN_USO));

            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return resultado;
        }

        private bool ValidarUsuarioRegistro(Modelo.Usuario usuarioNuevo)
        {
            if (String.IsNullOrEmpty(usuarioNuevo.Nombre))
            {
                throw new ArgumentException();
            }
            if (String.IsNullOrEmpty(usuarioNuevo.ContraseniaHASH))
            {
                throw new ArgumentException();
            }
            if (String.IsNullOrEmpty(usuarioNuevo.Correo))
            {
                throw new ArgumentException();
            }
            if (usuarioNuevo.FotoUsuario == null)
            {
                throw new ArgumentException();
            }
            return true;
        }

        public bool EsSha256Valido(string hash)
        {
            if (hash.Length != 64) return false;

            return hash.All(c => "0123456789abcdefABCDEF".Contains(c));
        }
    }
}
