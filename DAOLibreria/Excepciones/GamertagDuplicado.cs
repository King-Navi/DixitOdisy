using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Excepciones
{
    public class GamertagDuplicadoException : Exception
    {
        public GamertagDuplicadoException(string message) : base(message)
        {
        }

        public GamertagDuplicadoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
