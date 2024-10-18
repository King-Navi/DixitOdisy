using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioObtenerSesion
    {
        [TestClass]
        public class ManejadorPrincipalTests
        {
            [TestMethod]
            public void ObtenerSessionJugador_CuandoSesionNoExiste_DeberiaRegistrarSesionCorrectamente()
            {
                // Arrange
                var baseAddress = new Uri("net.pipe://localhost/ServicioUsuarioSesion");
                var binding = new NetNamedPipeBinding();
                var host = new ServiceHost(typeof(ServicioUsuarioSesionPrueba), baseAddress);
                host.AddServiceEndpoint(typeof(IServicioUsuarioSesion), binding, baseAddress);
                host.Open();

                var callbackInstance = new InstanceContext(new UsuarioSesionCallbackPrueba());
                var factory = new DuplexChannelFactory<IServicioUsuarioSesion>(callbackInstance, binding, new EndpointAddress(baseAddress));
                var proxy = factory.CreateChannel();

                var manejador = new ManejadorPrincipal();
                var usuario = new Usuario
                {
                    IdUsuario = 1,
                    Nombre = "UsuarioPrueba"
                };

                // Usar OperationContextScope para establecer el contexto
                using (new OperationContextScope((IContextChannel)proxy))
                {
                    // Act
                    manejador.ObtenerSessionJugador(usuario);

                    // Assert: verificr si el usuario fue agregado correctamente
                    Assert.IsTrue(manejador.YaIniciadoSesion(usuario.Nombre));

                    // verificamos si la sesión fue registrada correctamente a través del callback
                    var callback = (UsuarioSesionCallbackPrueba)callbackInstance.GetServiceInstance();
                    Assert.IsFalse(callback.SesionObtenida);
                }

                // Limpiar el entorno de prueba
                ((IClientChannel)proxy).Close();
                host.Close();
            }
        }
        //FIXME Nada de esto sirve 
        // Clase para agregar un callback simulado en el contexto
        public class CallbackContextExtension<T> : IExtension<OperationContext>
        {
            public T CallbackInstance { get; }

            public CallbackContextExtension(T callbackInstance)
            {
                CallbackInstance = callbackInstance;
            }

            public void Attach(OperationContext owner) { }

            public void Detach(OperationContext owner) { }
        }
        public class ServicioUsuarioSesionPrueba : IServicioUsuarioSesion
        {
            public void ObtenerSessionJugador(Usuario usuario)
            {
                // Este método estará vacío, ya que solo lo necesitamos como stub para crear el canal de proxy.
            }
        }
        public class UsuarioSesionCallbackPrueba : IUsuarioSesionCallback
        {
            public bool SesionObtenida { get; private set; }

            public void ObtenerSessionJugadorCallback(bool sesionAbierta)
            {
                SesionObtenida = sesionAbierta;
            }
        }
    }
}
