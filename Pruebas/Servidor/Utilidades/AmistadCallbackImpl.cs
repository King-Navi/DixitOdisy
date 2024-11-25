using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public class AmistadCallbackImpl : IAmistadCallBack , ICommunicationObject
    {
        ConcurrentDictionary<string, Amigo> amigos = new ConcurrentDictionary<string, Amigo>();
        public bool SesionAbierta { get; private set; }

        public CommunicationState State { get; private set; } = CommunicationState.Opened;

        public AmistadCallbackImpl()
        {
            SesionAbierta = false;
        }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;
        public void CambiarEstadoAmigo(Amigo amigo)
        {
            amigos.AddOrUpdate(amigo.Nombre,
                amigo,                  
                (clave, viejoValor) => amigo    
            );

        }

        public void EliminarAmigoCallback(Amigo amigo)
        {
            amigos.TryRemove(amigo.Nombre, out _);
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            amigos.TryAdd(amigo.Nombre, amigo);

        }


        public void Abort()
        {
            State = CommunicationState.Closed;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Close(TimeSpan timeout)
        {
            State = CommunicationState.Closed;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            State = CommunicationState.Closed;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Open(TimeSpan timeout)
        {
            State = CommunicationState.Opened;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            State = CommunicationState.Opened;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
