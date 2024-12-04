using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    public sealed class ServicioManejador<T> where T : ICommunicationObject, new()
    {

        public void EjecutarServicio(Action<T> action)
        {
            T cliente = new T();
            try
            {
                action(cliente); 
                cliente.Close(); 
            }
            catch (FaultException excepcionDeServicio)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeServicio);
            }
            catch (CommunicationException excepcionDeComunicacion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeComunicacion);
            }
            catch (TimeoutException excepcionDeTiempoExcedido)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeTiempoExcedido);
            }
            catch (Exception excepcionGeneral)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionGeneral);
            }
            finally
            {
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcionAbortar)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionAbortar);
                }
            }
        }

        public TResult EjecutarServicio<TResult>(Func<T, TResult> action)
        {
            T cliente = new T();
            try
            {
                TResult resultado = action(cliente); 
                cliente.Close(); 
                return resultado;
            }
            catch (FaultException excepcionDeServicio)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeServicio);
            }
            catch (CommunicationException excepcionDeComunicacion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeComunicacion);
            }
            catch (TimeoutException excepcionDeTiempoExcedido)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeTiempoExcedido);
            }
            catch (Exception excepcionGeneral)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionGeneral);
            }
            finally
            {
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcionAbortar)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionAbortar);
                }
            }
            return default;
        }

        public async Task EjecutarServicioAsync(Func<T, Task> action)
        {
            T cliente = new T();
            try
            {
                await action(cliente);
                cliente.Close();
            }
            catch (FaultException excepcionDeServicio)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeServicio);
            }
            catch (CommunicationException excepcionDeComunicacion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeComunicacion);
            }
            catch (TimeoutException excepcionDeTiempoExcedido)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeTiempoExcedido);
            }
            catch (Exception excepcionGeneral)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionGeneral);
            }
            finally
            {
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcionAbortar)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionAbortar);
                }
            }
        }

        public async Task<TResult> EjecutarServicioAsync<TResult>(Func<T, Task<TResult>> action)
        {
            T cliente = new T();
            try
            {
                TResult resultado = await action(cliente);
                cliente.Close();
                return resultado;
            }
            catch (FaultException excepcionDeServicio)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeServicio);
            }
            catch (CommunicationException excepcionDeComunicacion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeComunicacion);
            }
            catch (TimeoutException excepcionDeTiempoExcedido)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionDeTiempoExcedido);
            }
            catch (Exception excepcionGeneral)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionGeneral);
            }
            finally
            {
                try
                {
                    cliente.Abort();
                }
                catch (Exception excepcionAbortar)
                {
                    ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcionAbortar);
                }
            }
            return default;
        }
    }

}

