using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/communitytoolkit/mvvm/relaycommand</ref>
    public class ReleComado
    {
        private readonly Action accion; 
        private readonly Func<bool> puedeEjecutarse; 

        public ReleComado(Action _accion, Func<bool> _puedeEjecutarse = null)
        {
            accion = _accion;
            puedeEjecutarse = _puedeEjecutarse;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parametro)
        {
            return puedeEjecutarse == null || puedeEjecutarse();
        }

        public void Execute(object parametro)
        {
            accion();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
