using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Modelo
{
    internal class LecturaTrabajo
    {

        public string ArchivoPath { get; }
        public IPartidaCallback Callback { get; }
        public bool UsarGrupo { get; }

        public LecturaTrabajo(string archivoPath, IPartidaCallback callback, bool usarGrupo)
        {
            ArchivoPath = archivoPath;
            Callback = callback;
            UsarGrupo = usarGrupo;

        }
    }
}
