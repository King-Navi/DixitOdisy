using System.Globalization;
using System.Threading;
using System.Windows;

namespace WpfCliente
{
    public sealed class IdiomaGuardo
    {
        /// <summary>
        /// Guarda el idioma de la aplicación en español (mexico)
        /// </summary>
        public static void GuardarEspañolMX()
        { 
            var españolTag = Application.Current.Resources["EspañolTag"] as string;
            Properties.Settings.Default.codigoLenguaje = españolTag;
        }
        /// <summary>
        /// Guarda el idioma de la aplicación en ingles (EE.UU.)
        /// </summary>
        public static void GuardarInglesUS()
        {
            var englishTag = Application.Current.Resources["EnglishTag"] as string;
            Properties.Settings.Default.codigoLenguaje = englishTag;
        }
        /// <summary>
        /// Guarda el idioma de manera predetermianda en ingles (EE.UU.)
        /// </summary>
        public static void GuardarIdiomaPredeterminado()
        {
            var englishTag = Application.Current.Resources["EnglishTag"] as string;
            Properties.Settings.Default.codigoLenguaje = englishTag;
        }
        /// <summary>
        /// Carga el idioma guardado de Settings
        /// </summary>
        public static void CargarIdiomaGuardado()
        {
            string codigoLenguaje = WpfCliente.Properties.Settings.Default.codigoLenguaje;

            if (string.IsNullOrEmpty(codigoLenguaje))
            {
                var englishTag = Application.Current.Resources["EnglishTag"] as string;
                WpfCliente.Properties.Settings.Default.codigoLenguaje = englishTag;
                WpfCliente.Properties.Settings.Default.Save();
            }
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(codigoLenguaje);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(codigoLenguaje);
            }
            catch (CultureNotFoundException ex)
            {
                //TODO manejar error
                MessageBox.Show($"Error al cargar el idioma: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (CultureNotFoundException ex)
            {
                //TODO manejar el error
                MessageBox.Show($"Error al cambiar el idioma: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}

