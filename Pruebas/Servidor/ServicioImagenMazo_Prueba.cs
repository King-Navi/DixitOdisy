using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioImagenMazo_Prueba : ConfiguradorPruebaParaServicio
    {
        private ImagenMazoCallbackImplementacion ImagenCallbackImplementacion { get; set; } = new ImagenMazoCallbackImplementacion();
        private PartidaCallbackImplementacion implementacionPartidaCallback = new PartidaCallbackImplementacion();

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IImagenMazoCallback>()).Returns(ImagenCallbackImplementacion);

        }
        [TestCleanup]

        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
            ImagenCallbackImplementacion = null;
        }

        #region SolicitarImagenCarta
        [TestMethod]
        public async Task SolicitarImagenCarta_PartidaValida_DeberiaEnviarImagen()
        {
            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IImagenMazoCallback>()).Returns(ImagenCallbackImplementacion);

            await manejador.SolicitarImagenMazoAsync(idPartida);
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.IsTrue(ImagenCallbackImplementacion.ImagenCartasMazo.Count == 1);
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarVariasImagenes_DeberiaEnviarImagenes()
        {
            int numeroImagenes = 3;
            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IImagenMazoCallback>()).Returns(ImagenCallbackImplementacion);
            await manejador.SolicitarImagenMazoAsync(idPartida, numeroImagenes);
            await Task.Delay(TimeSpan.FromSeconds(10));

            Assert.IsTrue(ImagenCallbackImplementacion.ImagenCartasMazo.Count == 3);

            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarMuchasImagenes_DeberiaEnviarImagenes()
        {
            int tiempoEspera = 10;
            int cantidadRondas = 6;
            var numeroSolicitudes = 12;
            var usuario = new Usuario 
            { 
                IdUsuario = 19, 
                Nombre = "navi" 
            };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, cantidadRondas);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IImagenMazoCallback>()).Returns(ImagenCallbackImplementacion);
            var tareasSolicitudes = new List<Task>();
            await manejador.SolicitarImagenMazoAsync(idPartida, numeroSolicitudes);
            await Task.WhenAll(tareasSolicitudes);
            await Task.Delay(TimeSpan.FromSeconds(tiempoEspera));
            Assert.IsTrue(ImagenCallbackImplementacion.ImagenCartasMazo.Count == numeroSolicitudes);
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarImagenHastaDIOS_DeberiaEnviarImagenes()
        {
            string rutaCarpeta = null;
            //rutaCarpeta = Path.Combine("..", "..", "..", "WcfServicioLibreria", "Recursos", "Mitologia");
            if (!Directory.Exists(rutaCarpeta))
            {
                Console.WriteLine("Ruta completa: " + Path.GetFullPath(rutaCarpeta));
                Assert.Fail("El directorio de la WCFLibreria no se encontro");
            }
            string[] archivosJpg = Directory.GetFiles(rutaCarpeta, "*.jpg");
            Console.WriteLine("Imagenes en disco: " + archivosJpg.Length);

            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mitologia, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(contexto => contexto.GetCallbackChannel<IImagenMazoCallback>()).Returns(ImagenCallbackImplementacion);
            await manejador.SolicitarImagenMazoAsync(idPartida, archivosJpg.Length);

            await manejador.SolicitarImagenMazoAsync(idPartida);
            await Task.Delay(TimeSpan.FromSeconds(15));

            Assert.IsTrue(ImagenCallbackImplementacion.ImagenCartasMazo.Count == archivosJpg.Length +1);
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }


        #endregion
    }
}
