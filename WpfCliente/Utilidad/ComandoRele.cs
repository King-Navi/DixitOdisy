using System;
using System.Windows.Input;

namespace WpfCliente.Utilidad
{
    public class ComandoRele<T> : ICommand
    {
        private readonly Action<T> _executar;
        private readonly Func<T, bool> _seEjecuta;
        public event EventHandler CanExecuteChanged;

        public ComandoRele(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _executar = execute;
            _seEjecuta = canExecute;
        }

        public bool CanExecute(object parameter) => _seEjecuta == null || _seEjecuta((T)parameter);

        public void Execute(object parameter) => _executar((T)parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
