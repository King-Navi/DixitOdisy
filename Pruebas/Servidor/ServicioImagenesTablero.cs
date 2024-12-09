using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class ServicioImagenesTablero : ConfiguradorPruebaParaServicio
    {
        private ImagenTableroCallbackImplementacion ImagenCallbackImplementacion { get; set; } = new ImagenTableroCallbackImplementacion();
        private PartidaCallbackImplementacion implementacionPartidaCallback = new PartidaCallbackImplementacion();

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenesTableroCallback>()).Returns(ImagenCallbackImplementacion);

        }
        [TestCleanup]

        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
            ImagenCallbackImplementacion = null;
        }

        [TestMethod]
        public async Task SolicitarImagenMazoAsync_PartidaValida_DeberiaEnviarImagen()
        {
            var usuario = new Usuario 
            { 
                IdUsuario = 19, 
                Nombre = "navi" 
            };
            int numeroSolicitudes = 2;
            int tiempoEsperaSegundos = 2;
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(imitacion => imitacion.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(imitacion => imitacion.GetCallbackChannel<ImagenTableroCallbackImplementacion>()).Returns(ImagenCallbackImplementacion);
            await manejador.SolicitarImagenMazoAsync(usuario.Nombre, numeroSolicitudes);
            await Task.Delay(TimeSpan.FromSeconds(tiempoEsperaSegundos));
            var clavesImagenes=ImagenCallbackImplementacion.ImagenTablero.Keys.ToList();
            foreach (var nombreArchivo in clavesImagenes)
            {
                manejador.ConfirmarMovimiento(usuario.Nombre, idPartida, nombreArchivo);
            }
            await manejador.MostrarImagenesTablero(idPartida);
            await Task.Delay(TimeSpan.FromSeconds(tiempoEsperaSegundos));
            Assert.IsTrue(ImagenCallbackImplementacion.ImagenTablero.Count == clavesImagenes.Count);
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
    }
}
