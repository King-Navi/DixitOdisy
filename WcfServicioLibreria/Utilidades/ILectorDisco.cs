using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Utilidades
{
    public interface ILectorDisco
    {
        Task<bool> ProcesarColaLecturaEnvio(LecturaTrabajo lecturaTrabajo);
    }
}
