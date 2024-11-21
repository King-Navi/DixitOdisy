using System;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;

namespace Pruebas.Servidor.Utilidades
{
    public class UsuarioSesionCallbackImpl : IUsuarioSesionCallback, ICommunicationObject
    {
        public bool SesionAbierta { get; private set; }

        public CommunicationState State { get; private set; } = CommunicationState.Opened;

        public UsuarioSesionCallbackImpl()
        {
            SesionAbierta = false;
        }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

        public void ObtenerSessionJugadorCallback(bool sesionAbierta)
        {
            SesionAbierta = sesionAbierta;
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
