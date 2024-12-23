﻿using System.Globalization;
using System.Threading;
using System.Windows;
using WpfCliente.Utilidad;

namespace WpfCliente.Persistencia
{
    public sealed class IdiomaGuardo
    {
        private const string ESPANOL_TAG_MX = "EspañolTag";
        private const string ESPANOL_TAG_US = "EnglishTag";
        /// <summary>
        /// Guarda el idioma de la aplicación en español (mexico)
        /// </summary>
        public static void GuardarEspañolMX()
        { 
            var espanolTag = Application.Current.Resources[ESPANOL_TAG_MX] as string;
            Properties.Settings.Default.codigoLenguaje = espanolTag;
        }
        /// <summary>
        /// Guarda el idioma de la aplicación en ingles (EE.UU.)
        /// </summary>
        public static void GuardarInglesUS()
        {
            var englishTag = Application.Current.Resources[ESPANOL_TAG_US] as string;
            Properties.Settings.Default.codigoLenguaje = englishTag;
        }
        /// <summary>
        /// Guarda el idioma de manera predetermianda en ingles (EE.UU.)
        /// </summary>
        public static void GuardarIdiomaPredeterminado()
        {
            var englishTag = Application.Current.Resources[ESPANOL_TAG_US] as string;
            Properties.Settings.Default.codigoLenguaje = englishTag;
        }
        /// <summary>
        /// Carga el idioma guardado de Settings
        /// </summary>
        public static void CargarIdiomaGuardado()
        {
            string codigoLenguaje = Properties.Settings.Default.codigoLenguaje;

            if (string.IsNullOrEmpty(codigoLenguaje))
            {
                var englishTag = Application.Current.Resources[ESPANOL_TAG_US] as string;
                Properties.Settings.Default.codigoLenguaje = englishTag;
                Properties.Settings.Default.Save();
            }
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(codigoLenguaje);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(codigoLenguaje);
            }
            catch (CultureNotFoundException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }
        /// <summary>
        /// Cambia el idioma en tiempo de ejecucion
        /// </summary>
        /// <param name="lenguajeSelecionado"> Tiene que ser una tag valida del App.config</param>
        public static void SeleccionarIdioma(string lenguajeSelecionado)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(lenguajeSelecionado);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lenguajeSelecionado);
            }
            catch (CultureNotFoundException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
        }
    }
}

