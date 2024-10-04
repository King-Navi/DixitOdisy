using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
    }

}

