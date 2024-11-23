using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    public sealed class ServicioManejador<T> where T : ICommunicationObject, new()
    {
        /// <summary>
        /// Método para ejecutar acciones que no devuelven resultado (void)
        /// </summary>
        /// <param name="action"></param>
        public void EjecutarServicio(Action<T> action)
        {
            T cliente = new T();
            try
            {
                action(cliente); 
                cliente.Close(); 
            }
            catch
            {
                cliente.Abort(); 
            }
        }
        /// <summary>
        /// Método para ejecutar acciones que devuelven un resultado 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public TResult EjecutarServicio<TResult>(Func<T, TResult> action)
        {
            T cliente = new T();
            try
            {
                TResult resultado = action(cliente); 
                cliente.Close(); 
                return resultado;
            }
            catch(Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponentErrorException(excepcion);
                cliente.Abort(); 
            }
            return default;
        }
        /// <summary>
        /// Método para ejecutar acciones asíncronas que no devuelven resultado
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task EjecutarServicioAsync(Func<T, Task> action)
        {
            T cliente = new T();
            try
            {
                await action(cliente);
                cliente.Close();
            }
            catch
            {
                cliente.Abort();
                throw;
            }
        }

        /// <summary>
        /// Método para ejecutar acciones asíncronas que devuelven un resultado
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<TResult> EjecutarServicioAsync<TResult>(Func<T, Task<TResult>> action)
        {
            T cliente = new T();
            try
            {
                TResult resultado = await action(cliente);
                cliente.Close();
                return resultado;
            }
            catch
            {
                cliente.Abort();
                throw;
            }
        }
    }

}

