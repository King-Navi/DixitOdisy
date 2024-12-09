using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace WpfCliente.Utilidad
{
    public class CollecionObservableSeguraHilos<T> : ObservableCollection<T>
    {
        private const int SINCRONIZAR_INTERVALO_MILISEGUNDOS = 100;
        private readonly object sincronizarBloqueo = new object();
        private readonly Dispatcher dispatcher;
        private readonly ConcurrentQueue<T> memoriaEntrada = new ConcurrentQueue<T>();
        private readonly Timer sincronizarTemporizador;

        public CollecionObservableSeguraHilos() 
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            sincronizarTemporizador = new Timer(
                SincronizarConBuffer, 
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromMilliseconds(SINCRONIZAR_INTERVALO_MILISEGUNDOS));
        }
        public CollecionObservableSeguraHilos(IEnumerable<T> collection) : base(collection) 
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            sincronizarTemporizador = new Timer(
                SincronizarConBuffer, 
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromMilliseconds(SINCRONIZAR_INTERVALO_MILISEGUNDOS));
        }

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
                if (!EqualityComparer<T>.Default.Equals(elemento, default(T)))
                {
                    return this.Remove(elemento);
                }
                return false;
            }
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

        public void AgregarMemoriaEntrada(T item)
        {
            memoriaEntrada.Enqueue(item);
        }

        private void SincronizarConBuffer(object state)
        {
            if (memoriaEntrada.IsEmpty)
            {
                return;
            }
            try
            {
                dispatcher.Invoke(() =>
                {
                    lock (sincronizarBloqueo)
                    {
                        while (memoriaEntrada.TryDequeue(out var item))
                        {
                            Add(item);
                        }
                    }
                });
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {

            if (dispatcher.CheckAccess())
            {
                base.OnPropertyChanged(e);
            }
            else
            {
                dispatcher.Invoke(() => base.OnPropertyChanged(e));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (dispatcher.CheckAccess())
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                dispatcher.Invoke(() => base.OnCollectionChanged(e));
            }
        }
        public int ContarSeguro()
        {
            lock (sincronizarBloqueo)
            {
                return this.Count;
            }
        }
        public void Desechar()
        {
            sincronizarTemporizador.Dispose();
        }
    }

}
