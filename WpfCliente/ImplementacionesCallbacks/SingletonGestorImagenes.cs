using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class SingletonGestorImagenes
    {
            private static readonly Lazy<SingletonGestorImagenes> instancia = new Lazy<SingletonGestorImagenes>(() => new SingletonGestorImagenes());
            public ReceptorImagen imagnesMazo = new ReceptorImagen();
            public ReceptorImagen imagenesDeTodos = new ReceptorImagen();
            public static SingletonGestorImagenes Instancia => instancia.Value;
            private SingletonGestorImagenes() { }

            public void AbrirConexionImagenes()
            {
                imagenesDeTodos.AbrirConexionImagen();
                imagnesMazo.AbrirConexionImagen();
            }
            public void CerrarConexionImagenes()
            {
                imagenesDeTodos.CerrarConexionImagen();
                imagnesMazo.CerrarConexionImagen();
            }
        
    }
}
