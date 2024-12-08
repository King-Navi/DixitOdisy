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
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
                throw;
            }
        }

        private void Cerrado(object sender, EventArgs e)
        {
            Cerrar(sender);
        }

        private void EnFalla(object sender, EventArgs e)
        {
            Cerrar(sender);
        }

        private void Cerrar(object sender)
        {
            DesuscribirEventos((ICommunicationObject)sender);
            observador?.DesconectarUsuarioAsync(clavePropietario);
        }

        private void DesuscribirEventos(ICommunicationObject communicationObject)
        {
            if (!desechado && communicationObject != null)
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

                objetoComunicacion = null;
                observador = null;
                clavePropietario = null;
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
