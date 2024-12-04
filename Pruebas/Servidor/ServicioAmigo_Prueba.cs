using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using WcfServicioLibreria.Contratos;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioAmigo_Prueba : ConfiguradorPruebaParaServicio
    {
        private const int ID_USUARIO_MENOR = 1;
        private const int ID_USUARIO_MAYOR = 2;
        private const string NOMBRE_ID_MENOR = "NaviKing";
        private const string NOMBRE_ID_MAYOR = "adasda";
        private const string CONTRASNIAHASH_ID_MENOR = "6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b";
        private const string CONTRASNIAHASH_ID_MAYOR = "C1DB496F4E2EBBDCDE8A97461D659AE24C5EC0DE25E96AC04E4C1ECD9421950C";

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitarVetoDAO.Setup(dao => dao.ExisteTablaVetoPorIdCuenta(It.IsAny<int>())).Returns(false);
            imitarVetoDAO.Setup(dao => dao.CrearRegistroVeto(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<bool>())).Returns(true);
            imitarVetoDAO.Setup(dao => dao.VerificarVetoPorIdCuenta(It.IsAny<int>())).Returns(true);
            imitarUsuarioDAO.Setup(dao => dao.ObtenerIdPorNombre(It.IsAny<string>())).Returns(1);

        }
        [TestCleanup]
        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
        }
      
        [TestMethod]
        public void AceptarSolicitudAmistad_CuandoIdsSonValidosYAmbosConectados_DeberiaRetornarTrue()
        {

            
            var implementacionCallbackAmistad = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallbackAmistad);
            var implementacionCallbackUsarioSeion = new Utilidades.UsuarioSesionCallbackImplementacion();

            mockContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
                               .Returns(implementacionCallbackUsarioSeion);

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


            
            bool resultado = manejador.AceptarSolicitudAmistad(ID_USUARIO_MAYOR,ID_USUARIO_MENOR);

            
            Assert.IsTrue(implementacionCallbackAmistad.SesionAbierta , "La sesion deberia estar abierta");
            Assert.IsTrue(resultado, "El método debería devolver true cuando ambos usuarios están conectados y la solicitud de amistad se acepta.");

        }


    }
}
