﻿using System.Windows;
using WpfCliente.GUI;

namespace WpfCliente.Utilidad
{
    public class VentanasEmergentes
    {

        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente)
        {
            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteErrorServidor(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorServidor;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorServidor;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );
            ventanaEmergente.Owner = window;  
            ventanaEmergente.ShowDialog();
        }

        public static void CrearVentanaEmergenteErrorBD(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorBaseDatos;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorBaseDatos;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }


        public static void CrearVentanaEmergenteErrorInesperado(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloErrorInesperado;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeErrorInesperado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaEmergenteLobbyNoEncontrado(Window window)
        {
            string tituloVentanaEmergente = Properties.Idioma.tituloLobbyNoEncontrado;
            string descripcionVentanaEmergente = Properties.Idioma.mensajeLobbyNoEncontrado;

            VentanaEmergente ventanaEmergente = new VentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }

    }
}