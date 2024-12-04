using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfCliente.Utilidad
{
    public class CollecionObservableSeguraHilos<T> : ObservableCollection<T>
    {
        private readonly object sincronizarBloqueo = new object();
        public CollecionObservableSeguraHilos() { }
        public CollecionObservableSeguraHilos(IEnumerable<T> collection) : base(collection) { }

        public TResult RealizarConsultaSegura<TResult>(Func<IEnumerable<T>, TResult> consulta)
        {
            lock (sincronizarBloqueo)
            {
                return consulta(this);
            }
        }

        public bool EliminarElementoPorCondicion(Func<T, bool> condicion)
        {
            lock (sincronizarBloqueo)
            {
                var elemento = this.FirstOrDefault(condicion);
                if (elemento != null)
                {
                    return this.Remove(elemento);
                }
                return false;
            }
        }

        private new IEnumerator<T> GetEnumerator()
        {
            throw new InvalidOperationException();
        }

        protected override void InsertItem(int index, T item)
        {
            lock (sincronizarBloqueo)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (sincronizarBloqueo)
            {
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (sincronizarBloqueo)
            {
                base.ClearItems();
            }
        }

        protected override void SetItem(int index, T item)
        {
            lock (sincronizarBloqueo)
            {
                base.SetItem(index, item);
            }
        }
    }

}
