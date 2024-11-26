using System;

namespace DAOLibreria.Excepciones
{
    /// <summary>
    /// Excepción personalizada para errores relacionados con solicitudes de amistad.
    /// </summary>
    public class SolicitudAmistadExcepcion : Exception
    {
        public bool ExisteAmistad { get; }
        public bool ExistePeticion { get; }

        public SolicitudAmistadExcepcion(bool existeAmistad, bool existePeticion)
            : base("Error en la solicitud de amistad.")
        {
            ExisteAmistad = existeAmistad;
            ExistePeticion = existePeticion;
        }

        public SolicitudAmistadExcepcion(string message, bool existeAmistad, bool existePeticion)
            : base(message)
        {
            ExisteAmistad = existeAmistad;
            ExistePeticion = existePeticion;
        }

        public SolicitudAmistadExcepcion(string message, Exception innerException, bool existeAmistad, bool existePeticion)
            : base(message, innerException)
        {
            ExisteAmistad = existeAmistad;
            ExistePeticion = existePeticion;
        }
    }
}
