using System;
using System.ServiceModel;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Evento
{
    public class DesconectorEventoManejador
    {
        private ICommunicationObject objetoComunicacion;
        private IObservador observador;
        private string clavePropietario;
        private bool desechado;

        public DesconectorEventoManejador(ICommunicationObject communicationObjecto, IObservador _observador, string _clavePropietario)
        {
            try
            {
                observador = _observador;
                clavePropietario = _clavePropietario;
                communicationObjecto.Closed += Cerrado;
                communicationObjecto.Faulted += EnFalla;
                desechado = false;
            }
            catch (Exception)
            {

                throw;
            }
        }   
        
        private void Cerrado(object sender, EventArgs e)
        {
            DesuscribirEventos((ICommunicationObject)sender);
            observador?.DesconectarUsuario(clavePropietario);
        }

        private void EnFalla(object sender, EventArgs e)
        {
            DesuscribirEventos((ICommunicationObject)sender);
            observador?.DesconectarUsuario(clavePropietario);
        }
        
        private void DesuscribirEventos(ICommunicationObject communicationObject)
        {
            if (!desechado && communicationObject !=null)
            {
                communicationObject.Closed -= Cerrado;
                communicationObject.Faulted -= EnFalla;
            }
            desechado = true;
        }
        public void Desechar()
        {
            try
            {
                DesuscribirEventos(objetoComunicacion);
                if (objetoComunicacion != null)
                {
                    objetoComunicacion = null;
                }
                if (observador != null)
                {
                    observador = null;
                }
                if (clavePropietario != null)
                {
                    clavePropietario = null;
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }

        }
        
    }
}
