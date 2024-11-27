using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.Contexto;
using WpfCliente.Interfaz;

namespace WpfCliente.ManejadorAplicacion
{
    public class VentanaEventoManejador
    {
        public Window Ventana {  get; private set; }
        public bool EnProcesoDeCierre { get; set; } = false; 
        private bool desechado;
        private readonly Ventana nombre;
        private readonly IObservadorVentana observador;
        private readonly object bloqueoObjecto = new object(); 
        public VentanaEventoManejador(Window _ventana , IObservadorVentana _observador , Ventana _nombre)
        {
            Ventana = _ventana ?? throw new ArgumentNullException(nameof(_ventana));
            Ventana.Closed += OnClosed;
            desechado = false;
            nombre = _nombre;
            observador = _observador ?? throw new ArgumentNullException(nameof(_observador));
        }
        private void OnClosed(object sender, EventArgs e)
        {
            lock (bloqueoObjecto) 
            {
                if (desechado) return; 
                desechado = true;
                observador?.EnCierre(nombre);
                Ventana.Closed -= OnClosed; 
            }
        }
    }
}
