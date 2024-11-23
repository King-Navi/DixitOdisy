using System.Windows;
using WpfCliente.GUI;
using WpfCliente.ServidorDescribelo;

namespace WpfCliente.Utilidad
{
    public class Correo
    {
        public static bool VerificarCorreo(string correo, Window ventana)
        {
            var manejadorServicio = new ServicioManejador<ServicioCorreoClient>();
            var resultado = manejadorServicio.EjecutarServicio(servicio => {
                return servicio.VerificarCorreo(new Usuario()
                {
                    ContraseniaHASH = null,
                    Correo = correo,
                    Nombre = null,
                    FotoUsuario = null
                });
            });
            if (resultado)
            {
                string codigoIngresado = AbrirVentanaModal(ventana);
                return manejadorServicio.EjecutarServicio(servicio =>
                {
                    return servicio.VerificarCodigo(codigoIngresado);
                });
            }
            else
            {
                return false;
            }
        }

        private static string AbrirVentanaModal(Window ventana)
        {
            string valorObtenido = null;
            VerificarCorreoModalWindow ventanaModal = new VerificarCorreoModalWindow();
            ventanaModal.Owner = ventana;
            bool? resultado = ventanaModal.ShowDialog();

            if (resultado == true)
            {
                valorObtenido = ventanaModal.ValorIngresado;
            }

            return valorObtenido;
        }

        public static bool VerificarCorreoConGamertag(string correo, string nombreUsaurio)
        {
            var manejadorServicio = new ServicioManejador<ServicioCorreoClient>();
            var resultado = manejadorServicio.EjecutarServicio(servicio => {
                return servicio.VerificarCorreoConGamertag(new Usuario()
                {
                    ContraseniaHASH = null,
                    Correo = correo.ToLower(),
                    Nombre = nombreUsaurio,
                    FotoUsuario = null
                });
            });
            return resultado;
        }
    }
}
