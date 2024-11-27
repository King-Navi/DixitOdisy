using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks 
{
    public class SingletonAmigos : IServicioAmistadCallback, IImplementacionCallback
    {
        private const string UTILIMA_CONEXION_CONECTADO = "";
        private static readonly Lazy<SingletonAmigos> instancia = new Lazy<SingletonAmigos>(() => new SingletonAmigos());
        public static SingletonAmigos Instancia => instancia.Value;
        public ServicioAmistadClient Amigos { get; private set; }
        public ObservableCollection<Amigo> ListaAmigos { get;  private set; } = new ObservableCollection<Amigo>();


        private SingletonAmigos() { }

        public bool AbrirConexion()
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
                    CerrarConexion();
                }
                Amigos = new ServicioAmistadClient(new System.ServiceModel.InstanceContext(this));
                LimpiarRecursos();
                return true;

            }
            catch (Exception excepcion)
            {
                CerrarConexion();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
        }

        private void LimpiarRecursos()
        {
            ListaAmigos.Clear();
        }

        public bool CerrarConexion()
        {
            try
            {
                if (Amigos != null)
                {
                    Amigos.Close();
                    Amigos = null;
                }
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
            amigo.BitmapImagen = Imagen.ConvertirStreamABitmapImagen(amigo.Foto);
            if (amigo != null)
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

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                return;
            }
            else
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

        public void EliminarAmigoCallback(Amigo amigo)
        {
            if (amigo == null)
            {
                return;
            }
            var amigoAEliminar = ListaAmigos.FirstOrDefault(busqueda =>
                busqueda.Nombre.Equals(amigo.Nombre, StringComparison.OrdinalIgnoreCase));

            if (amigoAEliminar != null)
            {
                ListaAmigos.Remove(amigoAEliminar);
            }
        }
    }
}
