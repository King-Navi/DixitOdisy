using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioCorreo
    {
        public async Task<bool> VerificarCorreoAsync(Usuario usuario)
        {
            try
            {
                VerificarUsuarioCorreoNoNulo(usuario);
                string correoUsuario = usuario.Correo;
                string codigo = GenerarCodigo();
                codigosVerificacion.AddOrUpdate(correoUsuario, (codigo, DateTime.UtcNow), 
                    (llave, valorViejo) => (codigo, DateTime.UtcNow));
                return await EnviarCorreoAsync(codigo, correoUsuario);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion) 
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public async Task<bool> EnviarCorreoAsync(string codigo, string correoUsuario)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(SERVIDOR_SMTP, PUERTO_SMTP))
                {
                    smtpClient.Credentials = new NetworkCredential(CORREO_DESCRIBELO, CONTRASENIA_CORREO_DESCRIBELO);
                    smtpClient.EnableSsl = true;

                    string asunto = "--------------------------";
                    string cuerpo = "-----> " + codigo + "<-----";
                    using (MailMessage mail = new MailMessage(CORREO_DESCRIBELO, correoUsuario, asunto, cuerpo))
                    {
                        await smtpClient.SendMailAsync(mail);
                    }
                }
                return true;
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        public bool VerificarCodigo(string codigoRecibido ,string correoUsuario)
        {
            if (String.IsNullOrEmpty(codigoRecibido) ||
                String.IsNullOrEmpty(correoUsuario))
            {
                return false;
            }
            if (codigosVerificacion.TryRemove(correoUsuario, out var codigoGuardado))
            {
                return codigoGuardado.Codigo == codigoRecibido;
            }
            return false;
        }

        public bool VerificarCorreoConGamertag(Usuario usuario)
        {
            try
            {
                VerificarUsuarioCorreoNoNulo(usuario);
                return usuarioCuentaDAO.ExisteUnicoUsuarioConGamertagYCorreo(usuario.Correo, usuario.Nombre);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                return false;
            }
        }

        private static void EliminarCodigosExpirados(object state)
        {
            var ahora = DateTime.UtcNow;
            foreach (var codigo in codigosVerificacion)
            {
                if ((ahora - codigo.Value.Creacion).TotalMinutes >= TIEMPO_EXPIRACION_CODIGO_SEGUNDOS)
                {
                    codigosVerificacion.TryRemove(codigo.Key, out _);
                }
            }
        }

        private void VerificarUsuarioCorreoNoNulo(Usuario usuario)
        {
            if (usuario == null || String.IsNullOrEmpty(usuario.Correo))
            {
                throw new ArgumentNullException(nameof(usuario));
            }
        }

        private string GenerarCodigo()
        {
            string codigo = Utilidad.Generar6Caracteres();
            return codigo;
        }
    }
}
