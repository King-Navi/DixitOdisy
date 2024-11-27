using System;

namespace DAOLibreria.Excepciones
{
    public class GamertagDuplicadoException : Exception
    {
        public GamertagDuplicadoException() { }
        public GamertagDuplicadoException(string message) : base(message) { }
        public GamertagDuplicadoException(string message, Exception innerException) : base(message, innerException) { }
    }
}
