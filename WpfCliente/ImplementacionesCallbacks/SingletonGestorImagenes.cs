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
        public const int MAXIMO_IMAGENES_MAZO = 6;
        private static readonly Lazy<SingletonGestorImagenes> instancia = new Lazy<SingletonGestorImagenes>(() => new SingletonGestorImagenes());
        public ReceptorImagenMazo imagnesMazo = new ReceptorImagenMazo();
        public ReceptorImagenesTablero imagenesTablero = new ReceptorImagenesTablero();
        public static SingletonGestorImagenes Instancia => instancia.Value;
        private SingletonGestorImagenes() { }

        public void AbrirConexionImagenes()
        {
            imagenesTablero.AbrirConexionImagen();
            imagnesMazo.AbrirConexionImagen();
        }
        public void CerrarConexionImagenes()
        {
            imagenesTablero.CerrarConexionImagen();
            imagnesMazo.CerrarConexionImagen();
        }

        public static void PeticionImagenesHilo()
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await SingletonGestorImagenes.Instancia.imagnesMazo.Imagen.SolicitarImagenMazoAsync(SingletonCliente.Instance.IdPartida , 6);
                    }
                    catch (Exception excepcion)
                    {
                        ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                    }
                });
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }

        }
    }
}
