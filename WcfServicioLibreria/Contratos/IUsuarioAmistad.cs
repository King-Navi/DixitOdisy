using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Contratos
{
    public interface IUsuarioAmistad
    {
        [DataMember]
        IServicioPeticionAmistadCallBack PeticionAmistadCallBack { get; set; }
        [DataMember]
        EventHandler RecibioSolicitud { get; set; }
    }
}
