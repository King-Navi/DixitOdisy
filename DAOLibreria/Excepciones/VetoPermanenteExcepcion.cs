using System;

namespace DAOLibreria.Excepciones
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions#define-exception-classes</ref>
    public class VetoPermanenteExcepcion : Exception
    { 
        public VetoPermanenteExcepcion() { }
        public VetoPermanenteExcepcion(string message) : base(message) { }
        public VetoPermanenteExcepcion(string message, Exception innerException) : base(message, innerException) { }
    }
}
