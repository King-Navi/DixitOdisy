using System;

namespace WpfCliente.Utilidad
{
    /// <summary>
    /// La clase CambiarIdioma se encarga de manejar los eventos relacionados con el cambio de idioma en la aplicación.
    /// Notifica a todos los suscriptores cuando se ha realizado un cambio de idioma.
    /// </summary>
    public sealed class CambiarIdioma
    {
        /// <summary>
        /// Evento que se dispara cuando el idioma de la aplicación ha cambiado.
        /// Los suscriptores de este evento pueden realizar acciones como actualizar la interfaz de usuario con el nuevo idioma.
        /// </summary>
        public static event EventHandler LenguajeCambiado;

        /// <summary>
        /// Método que invoca el evento LenguajeCambiado, notificando a todos los suscriptores que el idioma ha cambiado.
        /// Este método debe ser llamado cuando el idioma de la aplicación se cambia.
        /// </summary>
        public static void EnCambioIdioma()
        {
            LenguajeCambiado?.Invoke(null, EventArgs.Empty);
        }
    }
}
