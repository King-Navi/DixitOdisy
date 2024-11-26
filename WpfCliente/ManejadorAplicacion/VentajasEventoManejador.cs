using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCliente.ManejadorAplicacion
{
    public class VentanaEventoManejador
    {
        public Window Ventana {  get; private set; }
        private bool desechado;
        public VentanaEventoManejador(Window _ventana)
        {
            Ventana = _ventana ?? throw new ArgumentNullException(nameof(_ventana));
            Ventana.Closed += OnClosed;
            desechado = false;
        }
        private void OnClosed(object sender, EventArgs e)
        {
            if (!desechado)
            {
                DesuscribirEventos();
            }
        }
        public void DesuscribirEventos()
        {
            if (!desechado)
            {
                Ventana.Closed -= OnClosed;
                desechado = true;
            }
        }
    }
}
