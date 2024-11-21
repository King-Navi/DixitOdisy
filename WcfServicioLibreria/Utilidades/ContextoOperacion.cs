using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
