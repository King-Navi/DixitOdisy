using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.ServidorDescribelo
{
    public partial class ChatMensaje
    {
        public override string ToString()
        {
            return $"{HoraFecha.ToLocalTime()} {Nombre} : {Mensaje}";
        }
    }
}
