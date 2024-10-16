using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {
        //TODO faltan añadir recursos de los errores
        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente)
        {
            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteErrorBD()
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorBaseDatos;
            string descripcionVentanaEmergente = Properties.Idioma.errorBaseDatos;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }


        public static void CrearVentanaEmergenteErrorInesperado()
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorInesperado;
            string descripcionVentanaEmergente = Properties.Idioma.errorInesperado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteLobbyNoEncontrado()
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorLobbyNoEncontrado;
            string descripcionVentanaEmergente = Properties.Idioma.errorLobbyNoEncontrado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

    }
}
