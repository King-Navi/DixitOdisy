﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ServicioImagen_Prueba : ConfiguradorPruebaParaServicio
    {
        private ImagenCallbackImplementacion ImagenCallbackImplementacion { get; set; } = new ImagenCallbackImplementacion();
        private PartidaCallbackImplementacion implementacionPartidaCallback = new PartidaCallbackImplementacion();

        [TestInitialize]
        public override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenCallback>()).Returns(ImagenCallbackImplementacion);

        }
        [TestCleanup]

        public override void LimpiadorTodo()
        {
            base.LimpiadorTodo();
            ImagenCallbackImplementacion = null;
        }

        #region SolicitarImagen
        [TestMethod]
        public async Task SolicitarImagenCarta_PartidaValida_DeberiaEnviarImagen()
        {
            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenCallback>()).Returns(ImagenCallbackImplementacion);

            await manejador.SolicitarImagenCartaAsync(idPartida);
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

            //Precaucion: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP 
            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenCallback>()).Returns(ImagenCallbackImplementacion);
            await manejador.SolicitarImagenCartaAsync(idPartida);
            await manejador.SolicitarImagenCartaAsync(idPartida);
            await manejador.SolicitarImagenCartaAsync(idPartida);
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
            //Precaucion: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP 
            var usuario = new Usuario { IdUsuario = 19, Nombre = "navi" };
            var configuracionGenerica = new ConfiguracionPartida(TematicaPartida.Mixta, CondicionVictoriaPartida.PorCantidadRondas, 6);
            var idPartida = manejador.CrearPartida(usuario.Nombre, configuracionGenerica);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenCallback>()).Returns(ImagenCallbackImplementacion);
            var tareasSolicitudes = new List<Task>();
            var numeroSolicitudes = 12;
            for (int i = 0; i < numeroSolicitudes; i++)
            {
                tareasSolicitudes.Add(manejador.SolicitarImagenCartaAsync(idPartida));
            }

            await Task.WhenAll(tareasSolicitudes);
            await Task.Delay(TimeSpan.FromSeconds(10));

            Assert.IsTrue(ImagenCallbackImplementacion.ImagenCartasMazo.Count == numeroSolicitudes);
            if (implementacionCallback != null)
            {
                implementacionCallback.Close();
            }
        }
        [TestMethod]
        public async Task SolicitarImagenCarta_SolicitarImagenHastaDIOS_DeberiaEnviarImagenes()
        {

            //PRECAUCION: El metodo puede fallar sobretodo si necesita hacer una solicitud HTTP y escribir en disco
            //PRECAUCION: Este metodo gasta credito (DINERO REAL), solo para mostrar a profe descomentar la linea de abajo
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
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IPartidaCallback>()).Returns(implementacionPartidaCallback);
            await manejador.UnirsePartidaAsync(usuario.Nombre, idPartida);
            imitacionContextoProvedor.Setup(c => c.GetCallbackChannel<IImagenCallback>()).Returns(ImagenCallbackImplementacion);
            foreach (var archivo in archivosJpg)
            {
                await manejador.SolicitarImagenCartaAsync(idPartida);
            }
            await manejador.SolicitarImagenCartaAsync(idPartida);
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
