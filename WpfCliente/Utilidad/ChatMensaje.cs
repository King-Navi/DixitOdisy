using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
