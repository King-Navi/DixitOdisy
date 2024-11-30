using System;

namespace DAOLibreria.Excepciones
{
    public class VetoPermanenteExcepcion : Exception
    {
        public VetoPermanenteExcepcion() { }
        public VetoPermanenteExcepcion(string message) : base(message) { }
        public VetoPermanenteExcepcion(string message, Exception innerException) : base(message, innerException) { }
    }
}
