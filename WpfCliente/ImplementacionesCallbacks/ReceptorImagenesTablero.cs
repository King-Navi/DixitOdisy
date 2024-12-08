using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReceptorImagenesTablero : IServicioImagenesTableroCallback
    {
        private readonly object lockImagenCartasTodos = new object();
        private CollecionObservableSeguraHilos<ImagenCarta> imagenesTablero = new CollecionObservableSeguraHilos<ImagenCarta>();
        public CollecionObservableSeguraHilos<ImagenCarta> ImagenesTablero
        {
            get
            {
                lock (lockImagenCartasTodos)
                {
                    return imagenesTablero;
                }
            }
            set
            {
                lock (lockImagenCartasTodos)
                {
                    imagenesTablero = value;
                }
            }
        }
        private readonly object bloquedoImagen = new object();
        private readonly object lockCantidad = new object();

        private ServicioImagenesTableroClient imagen;
        public ServicioImagenesTableroClient Imagen
        {
            get
            {
                lock (bloquedoImagen)
                {
                    if (imagen == null
                        || (imagen.State != CommunicationState.Opened 
                        && imagen.State != CommunicationState.Opening))
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

        public ReceptorImagenesTablero()
        {
            AbrirConexionImagen();
        }
        public bool AbrirConexionImagen()
        {
            try
            {
                Imagen = null;
                Imagen = new ServicioImagenesTableroClient(new InstanceContext(this));
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

        public void RecibirGrupoImagenCallback(ImagenCarta[] imagenes)
        {
            if (imagenes == null)
            {
                return;
            }
            try
            {
                foreach (var imagen in imagenes)
                {
                    if (imagen == null)
                    {
                        continue;
                    }

                    ImagenesTablero.AgregarMemoriaEntrada(imagen);
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionFatalComponente(excepcion);
            }
        }
    }
}
