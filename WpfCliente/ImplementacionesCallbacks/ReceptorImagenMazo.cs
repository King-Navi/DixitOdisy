using System;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReceptorImagenMazo : IServicioImagenMazoCallback
    {
        private readonly object lockImagenCartasMazo = new object();
        private CollecionObservableSeguraHilos<ImagenCarta> imagenesMazo = new CollecionObservableSeguraHilos<ImagenCarta>();
        public CollecionObservableSeguraHilos<ImagenCarta> ImagenesMazo
        {
            get
            {
                lock (lockImagenCartasMazo)
                {
                    return imagenesMazo;
                }
            }
            set
            {
                lock (lockImagenCartasMazo)
                {
                    imagenesMazo = value;
                }
            }
        }

        private readonly object bloqueoImagen = new object();

        private ServicioImagenMazoClient imagen;
        public ServicioImagenMazoClient Imagen
        {
            get
            {
                lock (bloqueoImagen)
                {
                    if (imagen == null 
                        || (imagen.State != CommunicationState.Opened && imagen.State != CommunicationState.Opening))
                    {
                        if (imagen != null)
                        {
                            try
                            {
                                imagen.Abort();
                            }
                            catch (Exception excepcion)
                            {
                                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
                            }
                        }
                        AbrirConexionImagen();
                    }

                    return imagen;
                }
            }
            set
            {
                lock (bloqueoImagen)
                {
                    imagen = value;
                }
            }
        }

        public ReceptorImagenMazo()
        {
            AbrirConexionImagen();
        }
        public bool AbrirConexionImagen()
        {
            try
            {
                Imagen = null;
                Imagen = new ServicioImagenMazoClient(new InstanceContext(this));
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarConexionImagen();
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                return false;
            }

        }

        public bool CerrarConexionImagen()
        {
            lock (bloqueoImagen) 
            {
                try
                {
                    if (imagen != null)
                    {
                        imagen.Close();
                        imagen = null;
                        return true;
                    }
                }
                catch (Exception excepcion)
                {
                    imagen.Abort();
                    imagen = null;
                    ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
                }
                return false;
            }
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null)
            {
                return;
            }
            ImagenesMazo.AgregarMemoriaEntrada(imagen);

        }

        public void RecibirVariasImagenCallback(ImagenCarta[] imagenes)
        {
            if (imagenes == null)
            {
                return;
            }
            foreach (var imagen in imagenes)
            {
                if (imagen == null)
                {
                    continue;
                }

                ImagenesMazo.AgregarMemoriaEntrada(imagen);
            }
        }
    }
}
