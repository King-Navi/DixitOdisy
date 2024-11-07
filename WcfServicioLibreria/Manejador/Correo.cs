using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioCorreo
    {
        public static string codigo;
        public bool VerificarCorreo(Usuario usuario)
        {
            try
            {
                codigo = GenerarCodigo();
                string correoUsuario = usuario.Correo;
                Task.Run(() => EnviarCorreoAsync(codigo, correoUsuario));
                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return false;
            }
        }

        public string GenerarCodigo()
        {
            string codigo = Utilidad.GenerarIdUnico();
            return codigo;
        }
        public async Task EnviarCorreoAsync(string codigo, string correoUsuario)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(correo, contrasenia);
                smtpClient.EnableSsl = true;

                string asunto = "Codigo de verificación";
                string cuerpo = "Tu código es: " + codigo;
                using (MailMessage mail = new MailMessage(correo, correoUsuario, asunto, cuerpo))
                {
                    await smtpClient.SendMailAsync(mail);
                }
            }
        }

        public bool VerificarCodigo(string codigoRecibido)
        {
            Console.WriteLine($"Código esperado: '{codigo}', Código recibido: '{codigoRecibido}'");
            return codigo == codigoRecibido;
        }

        public bool VerificarCorreoConGamertag(Usuario usuario)
        {
            try
            {
                return DAOLibreria.DAO.UsuarioDAO.ExisteUnicoUsuarioConGamertagYCorreo(usuario.Correo, usuario.Nombre);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return false;
            }
        }
    }
}
