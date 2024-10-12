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

namespace WcfServicioLibreria.Modelo
{
    public class DesconectorEventoManejador
    {
        private readonly List<ICommunicationObject> objetosComunicacion = new List<ICommunicationObject>();
        private IObservadorSala observador;
        private string clavePropietario;
        public EventHandler CerradoEvento { get; set; }
        public EventHandler FalloEvento { get; set; }
        public EventHandler DesconexionManejadorEvento;

        public void EnDesconexion()
        {
            DesconexionManejadorEvento?.Invoke(null, new UsuarioDesconectadoEventArgs(clavePropietario, DateTime.Now));
        }

        public virtual void Desechar()
        {
            DesuscribirseDeEventos();

        }
        private void DesuscribirseDeEventos()
        {
            throw new NotImplementedException();
            //foreach (var communicationObject in objetosComunicacion)
            //{
            //    DesuscribirEvento(communicationObject);
            //}
            //objetosComunicacion.Clear();
        }

        public DesconectorEventoManejador(ICommunicationObject communicationObject, IObservadorSala _observador, string _clavePropietario)
        {
            observador = _observador;
            clavePropietario = _clavePropietario;
            //communicationObject.Closed += Cerrado;
            //communicationObject.Faulted += EnFalla;
            objetosComunicacion.Add(communicationObject);
        }
        //private void Cerrado(object sender, EventArgs e)
        //{
        //    Console.WriteLine(" se ha ido de la sala (Closed).");
        //    DesuscribirEvento((ICommunicationObject)sender);
        //    observador.Desconectar(clavePropietario);
        //}

        //private void EnFalla(object sender, EventArgs e)
        //{
        //    Console.WriteLine(" ha fallado (Faulted).");
        //    DesuscribirEvento((ICommunicationObject)sender);
        //    observador.Desconectar(clavePropietario);
        //}
        //private void DesuscribirEvento(ICommunicationObject communicationObject)
        //{
        //    communicationObject.Closed -= Cerrado;
        //    communicationObject.Faulted -= EnFalla;
        //}
        //public void DesuscribirTodos()
        //{
        //    foreach (var communicationObject in objetosComunicacion)
        //    {
        //        DesuscribirEvento(communicationObject);
        //    }
        //    objetosComunicacion.Clear();
        //}
    }
}
