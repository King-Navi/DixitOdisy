using System;
using System.ServiceModel;
using System.Threading;

namespace Pruebas.Servidor.Utilidades
{
    public class ICommunicationObjectImpl : ICommunicationObject
    {

        public CommunicationState State { get; private set; } = CommunicationState.Opened;
        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

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
        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return BeginClose(TimeSpan.FromSeconds(1), callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Close();
            return new CompletedAsyncResult(callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return BeginOpen(TimeSpan.FromSeconds(1), callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Open();
            return new CompletedAsyncResult(callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            if (!(result is CompletedAsyncResult))
                throw new ArgumentException("El resultado proporcionado no es válido para EndClose.");
        }

        public void EndOpen(IAsyncResult result)
        {
            if (!(result is CompletedAsyncResult))
                throw new ArgumentException("El resultado proporcionado no es válido para EndOpen.");
        }

        private class CompletedAsyncResult : IAsyncResult
        {
            private readonly AsyncCallback _callback;
            private readonly object _state;

            public CompletedAsyncResult(AsyncCallback callback, object state)
            {
                _callback = callback;
                _state = state;
                IsCompleted = true;
                _callback?.Invoke(this);
            }

            public bool IsCompleted { get; }
            public WaitHandle AsyncWaitHandle => null;
            public object AsyncState => _state;
            public bool CompletedSynchronously => true;
        }
    }

}
