using System;
using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Manejador;

namespace WcfServidor
{
    /// <summary>
    /// Si lee esto profe que sepa que estuvo muy dificil esto (no me baje puntos por el comentario).
    /// </summary>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/framework/wcf/samples/custom-service-host</ref>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.servicehost?view=netframework-4.8.1</ref>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.channels.communicationobject.onopening?view=netframework-4.8.1#system-servicemodel-channels-communicationobject-onopening</ref>
    internal class DescribeloServiceHost : ServiceHost
    {
        private readonly ManejadorPrincipal _manejadorPrincipal;

        public DescribeloServiceHost(ManejadorPrincipal manejadorPrincipal, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _manejadorPrincipal = manejadorPrincipal;

        }

        public List<string> jugadoresConectados()
        {
            List<string> strings = new List<string>();
            //TODO:
            //foreach (var item in _manejadorPrincipal.)
            //{

            //}

            return strings;
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

