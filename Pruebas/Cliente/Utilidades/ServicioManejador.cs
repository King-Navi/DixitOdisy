using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Pruebas.Cliente
{
    //NOTA: Esta clase tiene que ser la misma que la del cliente
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
                //TODO:Manejar el error
                cliente.Abort(); 
                throw; 
            }
        }
        /// <summary>
        /// Método para ejecutar acciones que devuelven un resultado 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns name="resultado"></returns>
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
                //TODO:Manejar el error
                cliente.Abort(); 
                throw; 
            }
        }
        /// <summary>
        /// Método para ejecutar acciones asíncronas que no devuelven resultado
        /// </summary>
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

