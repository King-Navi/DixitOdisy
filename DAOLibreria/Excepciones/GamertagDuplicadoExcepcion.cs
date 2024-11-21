using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Excepciones
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions#define-exception-classes</ref>
    public class GamertagDuplicadoException : Exception
    {
        public GamertagDuplicadoException() { }
        public GamertagDuplicadoException(string message) : base(message) { }
        public GamertagDuplicadoException(string message, Exception innerException) : base(message, innerException) { }
    }
}
