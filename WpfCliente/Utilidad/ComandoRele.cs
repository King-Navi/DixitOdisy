using System;
using System.Windows.Input;

namespace WpfCliente.Utilidad
{
    public class ComandoRele<T> : ICommand
    {
        private readonly Action<T> executar;
        private readonly Func<T, bool> seEjecuta;
        public event EventHandler CanExecuteChanged;

        public ComandoRele(Action<T> _executar, Func<T, bool> sePuedeEjecutar = null)
        {
            executar = _executar;
            seEjecuta = sePuedeEjecutar;
        }

        public bool CanExecute(object parametro) => seEjecuta == null || seEjecuta((T)parametro);

        public void Execute(object parametro) => executar((T)parametro);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
