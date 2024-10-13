using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Utilidades
{
    public interface IUsuarioAmistad
    {
        [DataMember]
        IServicioPeticionAmistadCallBack PeticionAmistadCallBack { get; set; }
        [DataMember]
        EventHandler RecibioSolicitud { get; set; }
    }
}
