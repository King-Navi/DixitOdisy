using System;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReceptorImagen : IServicioImagenCallback
    {
        private readonly object lockImagenCartasMazo = new object();
        private CollecionObservableSeguraHilos<ImagenCarta> imagenCartasMazo = new CollecionObservableSeguraHilos<ImagenCarta>();
        public CollecionObservableSeguraHilos<ImagenCarta> ImagenCartasMazo
        {
            get
            {
                lock (lockImagenCartasMazo)
                {
                    return imagenCartasMazo;
                }
            }
            set
            {
                lock (lockImagenCartasMazo)
                {
                    imagenCartasMazo = value;
                }
            }
        }

        private readonly object lockImagenCartasTodos = new object();
        private CollecionObservableSeguraHilos<ImagenCarta> imagenCartasTodos = new CollecionObservableSeguraHilos<ImagenCarta>();
        public CollecionObservableSeguraHilos<ImagenCarta> ImagenCartasTodos
        {
            get
            {
                lock (lockImagenCartasTodos)
                {
                    return imagenCartasTodos;
                }
            }
            set
            {
                lock (lockImagenCartasTodos)
                {
                    imagenCartasTodos = value;
                }
            }
        }
        private readonly object bloquedoImagen = new object();
        private readonly object lockCantidad = new object();

        private ServicioImagenClient imagen;
        public ServicioImagenClient Imagen
        {
            get
            {
                lock (bloquedoImagen)
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
                lock (bloquedoImagen)
                {
                    imagen = value;
                }
            }
        }

        public ReceptorImagen()
        {
            AbrirConexionImagen();
        }
        public bool AbrirConexionImagen()
        {
            try
            {
                Imagen = null;
                Imagen = new ServicioImagenClient(new InstanceContext(this));
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
            lock (bloquedoImagen) 
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

        public void RecibirGrupoImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null)
            {
                return;
            }
            ImagenCartasTodos.Add(imagen);
        }

        public void RecibirImagenCallback(ImagenCarta imagen)
        {
            if (imagen == null)
            {
                return;
            }
            ImagenCartasMazo.Add(imagen);
        }

        public int ObtenerCantidadCartasMazo()
        {
            lock (lockCantidad)
            {
                return ImagenCartasMazo.Count;
            }
        }

        public int ObtenerCantidadCartasTodos()
        {
            lock (lockCantidad)
            {
                return ImagenCartasTodos.Count;
            }
        }
    }
}
