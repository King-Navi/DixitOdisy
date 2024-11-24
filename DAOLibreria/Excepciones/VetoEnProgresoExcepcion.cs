using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Excepciones
{
    public class VetoEnProgresoExcepcion : Exception
    {
        public VetoEnProgresoExcepcion() { }
        public VetoEnProgresoExcepcion(string message) : base(message) { }
        public VetoEnProgresoExcepcion(string message, Exception innerException) : base(message, innerException) { }
    }
}
