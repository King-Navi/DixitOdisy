using DAOLibreria;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioAmigo_Prueba 
    {
        private const int ID_USUARIO_MENOR = 1;
        private const int ID_USUARIO_MAYOR = 2;
        private const string NOMBRE_ID_MENOR = "NaviKing";
        private const string NOMBRE_ID_MAYOR = "adasda";
        private const string CONTRASNIAHASH_ID_MENOR = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
        private const string CONTRASNIAHASH_ID_MAYOR = "C1DB496F4E2EBBDCDE8A97461D659AE24C5EC0DE25E96AC04E4C1ECD9421950C";
        private const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";

        protected Mock<IContextoOperacion> mockContextoProvedor;
        protected ManejadorPrincipal manejador;

        [TestInitialize]
        public virtual void PruebaConfiguracion()
        {
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
            mockContextoProvedor = new Mock<IContextoOperacion>();
            manejador = new ManejadorPrincipal(mockContextoProvedor.Object);
        }
        [TestCleanup]
        public void PruebaLimpieza()
        {
            manejador.CerrarAplicacion();
            mockContextoProvedor = null;
            manejador = null;
            using (var context = new DescribeloEntities())
            {
                var amistades = context.Amigo
                                       .Where(amistad => (amistad.idMayor_usuario == ID_USUARIO_MENOR || amistad.idMenor_usuario == ID_USUARIO_MENOR) ||
                                                   (amistad.idMayor_usuario == ID_USUARIO_MAYOR || amistad.idMenor_usuario == ID_USUARIO_MAYOR))
                                       .ToList();
                context.Amigo.RemoveRange(amistades);
                context.SaveChanges();
            }
            using (var context = new DescribeloEntities())
            {
                var solicitudes = context.PeticionAmistad
                                         .Where(solicitud => solicitud.idRemitente == ID_USUARIO_MENOR ||
                                                             solicitud.idDestinatario == ID_USUARIO_MENOR ||
                                                             solicitud.idRemitente == ID_USUARIO_MAYOR ||
                                                             solicitud.idDestinatario == ID_USUARIO_MAYOR)
                                         .ToList();
                context.PeticionAmistad.RemoveRange(solicitudes);
                context.SaveChanges();
            }
        }

        private static void AgregarAmistad(int idMayor, int idMenor)
        {
            using (var context = new DescribeloEntities())
            {
                var amistad = new Amigo
                {
                    idMayor_usuario = Math.Max(idMayor, idMenor),
                    idMenor_usuario = Math.Min(idMayor, idMenor)
                };
                context.Amigo.Add(amistad);
                context.SaveChanges();
            }
        }
        public void GuardarSolicitudAmistad(int idRemitente, int idDestinatario)
        {

            using (var context = new DescribeloEntities())
            {
                var nuevaSolicitud = new PeticionAmistad
                {
                    idRemitente = idRemitente,
                    idDestinatario = idDestinatario,
                    fechaPeticion = DateTime.Now,
                    estado = ESTADO_SOLICITUD_PENDIENTE
                };

                context.PeticionAmistad.Add(nuevaSolicitud);
                context.SaveChanges();
            }
        }

      
        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoIdsSonValidosYAmbosConectados_DeberiaRetornarTrue()
        {

            // Arrange
            var implementacionCallbackAmistad = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallbackAmistad);
            var implementacionCallbackUsarioSeion = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextProvider => contextProvider.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallbackUsarioSeion);

            // Agregar ambos usuarios a la lista de jugadores conectados deben esta en BD
            GuardarSolicitudAmistad(ID_USUARIO_MAYOR, ID_USUARIO_MENOR);
            manejador.ObtenerSesionJugador(new WcfServicioLibreria.Modelo.Usuario()
            {
                IdUsuario = ID_USUARIO_MENOR,
                Nombre = NOMBRE_ID_MENOR,
                ContraseniaHASH = CONTRASNIAHASH_ID_MENOR
            });

            manejador.ObtenerSesionJugador(new WcfServicioLibreria.Modelo.Usuario()
            {
                IdUsuario =ID_USUARIO_MAYOR,
                Nombre = NOMBRE_ID_MAYOR,
                ContraseniaHASH = CONTRASNIAHASH_ID_MAYOR
            });


            // Act
            bool resultado = manejador.AceptarSolicitudAmistad(ID_USUARIO_MAYOR,ID_USUARIO_MENOR);

            // Assert
            Assert.IsTrue(resultado, "El método debería devolver true cuando ambos usuarios están conectados y la solicitud de amistad se acepta.");
        }


    }
}
