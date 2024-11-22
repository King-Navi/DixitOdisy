using System.ServiceModel;

namespace WcfServicioLibreria.Utilidades
{
    public class ContextoOperacion : IContextoOperacion
    {
        public T GetCallbackChannel<T>()
        {
            return OperationContext.Current.GetCallbackChannel<T>();
        }
    }
}
