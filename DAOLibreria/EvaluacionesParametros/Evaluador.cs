using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.EvaluacionesParametros
{
    internal static class Evaluador
    {
        public static void EvaluarIdsNoSonIguales(int idUsuarioRemitente, int idUsuarioDestinatario)
        {
            if (idUsuarioRemitente == idUsuarioDestinatario)
            {
                throw new ArgumentException();
            }
        }
    }
}
