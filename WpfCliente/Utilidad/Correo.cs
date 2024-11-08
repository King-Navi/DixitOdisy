using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.GUI;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class Correo
    {
        public static bool VerificarCorreo(string correo, Window window)
        {
            var manejadorServicio = new ServicioManejador<ServicioCorreoClient>();
            var resultado = manejadorServicio.EjecutarServicio(proxy => {
                return proxy.VerificarCorreo(new Usuario()
                {
                    ContraseniaHASH = null,
                    Correo = correo,
                    Nombre = null,
                    FotoUsuario = null
                });
            });
            if (resultado)
            {
                string codigoIngresado = AbrirVentanaModal(window);
                return manejadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.VerificarCodigo(codigoIngresado);
                });
            }
            else
            {
                return false;
            }
        }

        private static string AbrirVentanaModal(Window window)
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
            ventanaModal.Owner = window;
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static bool VerificarCorreoConGamertag(string correo, string gamertag)
        {
            var manejadorServicio = new ServicioManejador<ServicioCorreoClient>();
            var resultado = manejadorServicio.EjecutarServicio(proxy => {
                return proxy.VerificarCorreoConGamertag(new Usuario()
                {
                    ContraseniaHASH = null,
                    Correo = correo.ToLower(),
                    Nombre = gamertag,
                    FotoUsuario = null
                });
            });
            return resultado;
        }
    }
}
