using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente
{
    public interface IActualizacionUI
    {
        void LenguajeCambiadoManejadorEvento(object sender, EventArgs e);
        void ActualizarUI();

    }
}
