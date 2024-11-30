using System;

namespace DAOLibreria.Excepciones
{
    public class ActividadSospechosaExcepcion : Exception
    {
        public int Identificador { get; set; }
        public ActividadSospechosaExcepcion() { }
        public ActividadSospechosaExcepcion(string message) : base(message) { }
        public ActividadSospechosaExcepcion(string message, Exception innerException) : base(message, innerException) { }

    }
}
