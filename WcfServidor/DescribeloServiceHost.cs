using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Manejador;

namespace WcfServidor
{
    internal class DescribeloServiceHost : ServiceHost
    {
        private readonly ManejadorPrincipal manejadorPrincipal;

        public DescribeloServiceHost(ManejadorPrincipal _manejadorPrincipal, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this.manejadorPrincipal = _manejadorPrincipal;

        }
        protected override void OnClosing()
        {
            manejadorPrincipal.CerrarAplicacion();
            base.OnClosing();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            Console.WriteLine("Servidor atrapado!!!");
        }
    }
}

