using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class SingletonGestorImagenes
    {
        private const int MAXIMO_IMAGENES_MAZO = 6;
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

        public void PeticionImagenesHilo()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        for (int i = 0; i < MAXIMO_IMAGENES_MAZO; i++)
                        {
                            SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenCartaAsync(SingletonCliente.Instance.IdPartida);
                        }
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                    }
                });
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
           
        }
    }
}
