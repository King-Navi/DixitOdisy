using System;

namespace DAOLibreria.Excepciones
{
    public class VetoEnProgresoExcepcion : Exception
    {
        public VetoEnProgresoExcepcion() { }
        public VetoEnProgresoExcepcion(string message) : base(message) { }
        public VetoEnProgresoExcepcion(string message, Exception innerException) : base(message, innerException) { }
    }
}
