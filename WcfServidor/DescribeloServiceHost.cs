using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Manejador;

namespace WcfServidor
{
    internal class DescribeloServiceHost : ServiceHost
    {
        private readonly ManejadorPrincipal _manejadorPrincipal;

        public DescribeloServiceHost(ManejadorPrincipal manejadorPrincipal, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _manejadorPrincipal = manejadorPrincipal;

        }

        public List<string> JugadoresConectados()
        {
            _manejadorPrincipal.JugadoresConectado();

            return null;
        }

        protected override void OnClosing()
        {
            _manejadorPrincipal.CerrarAplicacion(); 
            
            base.OnClosing();
        }

        protected override void OnClosed()
        {
            Console.WriteLine("Servidor atrapado!!!");
            base.OnClosed();
        }
    }
}

