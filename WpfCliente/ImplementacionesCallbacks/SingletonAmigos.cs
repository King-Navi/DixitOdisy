using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks 
{
    public partial class SingletonCanal : IServicioAmistadCallback
    {
        private const string UTILIMA_CONEXION_CONECTADO = "";
        public ServicioAmistadClient Amigos { get; private set; }
        private readonly object listaAmigoBloqueo = new object();
        public CollecionObservableSeguraHilos<Amigo> ListaAmigos { get; private set; } = new CollecionObservableSeguraHilos<Amigo>();
        private void LimpiarRecursosAmigos()
        {
            try
            {
                lock (listaAmigoBloqueo)
                {
                    ListaAmigos?.Clear(); 
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        public bool AbrirConexionAmistad()
        {
            try
            {
                var objectoComunicacion = Amigos as ICommunicationObject;
                if (objectoComunicacion?.State == CommunicationState.Opened ||
                    objectoComunicacion?.State == CommunicationState.Opening)
                {
                    return true;
                }
                if (objectoComunicacion?.State == CommunicationState.Faulted)
                {
                    CerrarConexionAmigos();
                }
                Amigos = new ServicioAmistadClient(new System.ServiceModel.InstanceContext(this));
                return true;
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                CerrarConexionAmigos();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;

        }


        public bool CerrarConexionAmigos()
        {
            try
            {
                if (Amigos != null)
                {
                    Amigos.Close();
                    Amigos = null;
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                Amigos = null;
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
            return true;
        }

        public void CambiarEstadoAmigoCallback(Amigo amigo)
        {
            amigo.BitmapImagen = Imagen.ConvertirBytesABitmapImage(amigo.Foto);
            if (amigo != null)
            {
                lock (listaAmigoBloqueo)
                {
                    if (amigo.Estado == EstadoAmigo.Conectado)
                    {
                        var amigoAEliminar = ListaAmigos.FirstOrDefault(busqueda =>
                            busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));
                        if (amigoAEliminar != null)
                        {
                            ListaAmigos.Remove(amigoAEliminar);
                            amigo.UltimaConexion = UTILIMA_CONEXION_CONECTADO;
                            amigo.EstadoActual = Properties.Idioma.labelConectado;
                            ListaAmigos.Insert(0, amigo);
                        }
                    }
                    else
                    {
                        var amigoAEliminar = ListaAmigos.FirstOrDefault(busqueda =>
                            busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));
                        if (amigoAEliminar != null)
                        {
                            ListaAmigos.Remove(amigoAEliminar);
                            amigo.EstadoActual = Properties.Idioma.labelDesconectado;
                            ListaAmigos.Insert(0, amigo);
                        }
                    } 
                }
            }
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            try
            {
                if (amigo == null)
                {
                    return;
                }
                else
                {
                    lock (listaAmigoBloqueo)
                    {
                        if (amigo.Estado == EstadoAmigo.Desconectado)
                        {
                            amigo.EstadoActual = Properties.Idioma.labelDesconectado;
                        }
                        if (amigo.Estado == EstadoAmigo.Conectado)
                        {
                            amigo.EstadoActual = Properties.Idioma.labelConectado;
                            amigo.UltimaConexion = UTILIMA_CONEXION_CONECTADO;

                        }
                        ListaAmigos.Add(amigo);
                    }

                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }

        public void EliminarAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                return;
            }
            try
            {
                lock (listaAmigoBloqueo)
                {
                    var amigoAEliminar = ListaAmigos.FirstOrDefault(busqueda =>
                                    busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));

                    if (amigoAEliminar != null)
                    {
                        ListaAmigos.Remove(amigoAEliminar);
                    } 
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
        }
    }
}
