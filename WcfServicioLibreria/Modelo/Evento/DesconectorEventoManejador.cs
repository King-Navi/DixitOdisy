using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Evento
{
    /// <summary>
    /// Gestiona la desconexión y los fallos en los objetos de comunicación, asegurando que se notifique a los observadores
    /// apropiados y se desuscriba de los eventos pertinentes.
    /// </summary>
    public class DesconectorEventoManejador
    {
        private ICommunicationObject objetoComunicacion;
        private IObservador observador;
        private string clavePropietario;
        private bool desechado;
        /// <summary>
        /// Inicializa una nueva instancia de la clase con los componentes necesarios
        /// para gestionar eventos de comunicación.
        /// </summary>
        /// <param name="communicationObject"></param>
        /// <param name="_observador"></param>
        /// <param name="_clavePropietario"></param>
        public DesconectorEventoManejador(ICommunicationObject communicationObject, IObservador _observador, string _clavePropietario)
        {
            observador = _observador;
            clavePropietario = _clavePropietario;
            communicationObject.Closed += Cerrado;
            communicationObject.Faulted += EnFalla;
            desechado = false;
        }
        /// <summary>
        /// Maneja el evento de cierre de la comunicación, notificando al observador correspondiente y desuscribiendo los eventos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cerrado(object sender, EventArgs e)
        {
            Console.WriteLine(" se ha ido de la "+ sender.ToString()+ " (Closed).");
            DesuscribirEventos((ICommunicationObject)sender);
            observador.DesconectarUsuario(clavePropietario);
        }
        /// <summary>
        /// Maneja el evento de fallo en la comunicación, notificando al observador correspondiente y desuscribiendo los eventos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnFalla(object sender, EventArgs e)
        {
            Console.WriteLine(" ha fallado de la" + sender.ToString() + " (Faulted).");
            DesuscribirEventos((ICommunicationObject)sender);
            observador.DesconectarUsuario(clavePropietario);
        }
        /// <summary>
        /// Desuscribe los manejadores de eventos <see cref="Cerrado"/> y <see cref="EnFalla"/> del objeto de comunicación para evitar fugas de memoria.
        /// </summary>
        /// <param name="communicationObject">El objeto de comunicación del cual desuscribir los eventos.</param>
        private void DesuscribirEventos(ICommunicationObject communicationObject)
        {
            if (!desechado)
            {
                communicationObject.Closed -= Cerrado;
                communicationObject.Faulted -= EnFalla;
            }
            desechado = true;
        }
        public void Desechar()
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
        
    }
}
