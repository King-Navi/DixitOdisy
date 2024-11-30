using System;
using System.ServiceModel;

namespace WcfServicioLibreria.Utilidades
{
    public class ContextoOperacion : IContextoOperacion
    {
        public T GetCallbackChannel<T>()
        {
			try
			{
				return OperationContext.Current.GetCallbackChannel<T>();
			}
			catch (InvalidOperationException excecpion)
			{
				ManejadorExcepciones.ManejarErrorException(excecpion);
			}
			catch (Exception excecpion)
			{
                ManejadorExcepciones.ManejarErrorException(excecpion);
            }
			return default;
        }
    }
}
