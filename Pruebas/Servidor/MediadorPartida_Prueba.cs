using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class MediadorPartida_Prueba
    {
        #region ObtenerRutaCompeltaImagen
        [TestMethod]
        public void ObtenerRutaCompeltaImagen_ImagnesDisponibles_DevuelveRutaValida()
        {
            var tematica = TematicaPartida.Animales;
            var mediador = new MediadorPartida(tematica);
            var (rutaCompleta, nombreArchivo) = mediador.ObtenerRutaCompeltaYNombreImagen();
            Console.WriteLine(rutaCompleta);
            Assert.IsNotNull(rutaCompleta, "La ruta devuelta no debe ser nula.");
            Assert.IsNotNull(nombreArchivo, "La ruta devuelta no debe ser nula.");
            Assert.IsTrue(File.Exists(rutaCompleta), "La ruta devuelta debe corresponder a un archivo existente.");
        }


        [TestMethod]
        public void ObtenerRutaCompletaYNombreImagen_AccesoConcurrente_SinRepeticiones()
        {
            var mediador = new MediadorPartida(TematicaPartida.Mixta);
            int numeroImagenes = mediador.CartasRestantes;
            int numeroHilos = 10;
            int solicitudesPorHilo = numeroImagenes / numeroHilos;
            var resultados = new ConcurrentBag<string>();
            Parallel.For(0, numeroHilos, (i) =>
            {
                for (int j = 0; j < solicitudesPorHilo; j++)
                {
                    var resultado = mediador.ObtenerRutaCompeltaYNombreImagen();
                    if (!string.IsNullOrEmpty(resultado.NombreArchivo))
                    {
                        resultados.Add(resultado.NombreArchivo);
                    }
                }
            });
            var duplicados = resultados.GroupBy(r => r)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();
            Assert.AreEqual(numeroImagenes, resultados.Count, "La cantidad de imágenes retornadas no coincide con las solicitudes.");
            Assert.IsFalse(duplicados.Any(), "Se encontraron imágenes repetidas en el acceso concurrente.");
        }

        #endregion ObtenerRutaCompeltaImagen
        #region ObtenerMultiplesRutasYNombres
        [TestMethod]
        public void ObtenerMultiplesRutasYNombres_ImagnesDisponibles_DevuelveRutaValidaEspacio()
        {
            var tematica = TematicaPartida.Espacio;
            var mediador = new MediadorPartida(tematica);
            List < (string RutaCompleta, string NombreArchivo) > archivos = mediador.ObtenerMultiplesRutasYNombres(6);

            foreach (var ruta in archivos)
            {
                Assert.IsTrue(File.Exists(ruta.RutaCompleta), "La ruta devuelta debe corresponder a un archivo existente.");
            }
        }

        [TestMethod]
        public void ObtenerMultiplesRutasYNombres_ImagnesDisponibles_DevuelveRutaValidaMitologia()
        {
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<(string RutaCompleta, string NombreArchivo)> archivos = mediador.ObtenerMultiplesRutasYNombres(12);

            foreach (var ruta in archivos)
            {
                Assert.IsTrue(File.Exists(ruta.RutaCompleta), "La ruta devuelta debe corresponder a un archivo existente.");
            }
        }


        #endregion ObtenerMultiplesRutasYNombres
        #region

        [TestMethod]
        public void ObtenerRutasPorClave_RutasExistentes_RetornaRutasCorrectas()
        {
            int numeroArchivos = 12;
            string nombreUsuario = "UsuarioPruebas";
            string nombreInvitado = "uUSAIR invitado";
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<(string RutaCompleta, string NombreArchivo)> archivos = mediador.ObtenerMultiplesRutasYNombres(numeroArchivos);
            ConcurrentDictionary<string, List<string>> listaActualElegida = new ConcurrentDictionary<string, List<string>>();
            List<string> listaEleciones = new List<string>();
            foreach (var ruta in archivos)
            {
                listaActualElegida.AddOrUpdate(
                    nombreUsuario,
                    new List<string> { ruta.NombreArchivo },
                    (key, existingList) =>
                    {
                        existingList.Add(ruta.NombreArchivo);
                        return existingList;
                    }
                );
            }
            List<(string RutaCompleta, string NombreArchivo)> archivosDeInvitado = mediador.ObtenerMultiplesRutasYNombres(numeroArchivos);
            foreach (var ruta in archivos)
            {
                listaActualElegida.AddOrUpdate(
                    nombreInvitado,
                    new List<string> { ruta.NombreArchivo },
                    (key, existingList) =>
                    {
                        existingList.Add(ruta.NombreArchivo);
                        return existingList;
                    }
                );
            }
            var todasLasImagenesGrupo = listaActualElegida.Values.SelectMany(lista => lista).ToList();

            List<string> resultado = mediador.ObtenerRutasPorNombreArchivo(todasLasImagenesGrupo);
            Assert.AreEqual(numeroArchivos + numeroArchivos, resultado.Count);
        }

        [TestMethod]
        public void ObtenerRutasPorNombreArchivo_TodosLosArchivosExisten_RetornaRutasCompletas()
        {
            int numeroArchivos = 12;
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<(string RutaCompleta, string NombreArchivo)> archivos = mediador.ObtenerMultiplesRutasYNombres(numeroArchivos);
            List<string> listaNombres = archivos.Select(a => a.NombreArchivo).ToList();
            List<string> resultado = mediador.ObtenerRutasPorNombreArchivo(listaNombres);
            Assert.AreEqual(numeroArchivos, resultado.Count, "La cantidad de rutas retornadas no coincide con el número de archivos esperados.");
            CollectionAssert.AreEquivalent(archivos.Select(a => a.RutaCompleta).ToList(), resultado, "Las rutas retornadas no son las esperadas.");
        }

        [TestMethod]
        public void ObtenerRutasPorNombreArchivo_AlgunosArchivosNoExisten_RetornaSoloRutasValidas()
        {
            int numeroArchivos = 12;
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<(string RutaCompleta, string NombreArchivo)> archivos = mediador.ObtenerMultiplesRutasYNombres(numeroArchivos);
            List<string> listaNombres = archivos.Select(a => a.NombreArchivo).ToList();
            listaNombres.Add("archivoInexistente1");
            listaNombres.Add("archivoInexistente2");
            List<string> resultado = mediador.ObtenerRutasPorNombreArchivo(listaNombres);
            Assert.AreEqual(numeroArchivos, resultado.Count, "La cantidad de rutas retornadas no coincide con el número de archivos válidos esperados.");
            CollectionAssert.AreEquivalent(archivos.Select(a => a.RutaCompleta).ToList(), resultado, "Las rutas retornadas no son las esperadas.");
        }

        [TestMethod]
        public void ObtenerRutasPorNombreArchivo_NingunArchivoExiste_RetornaListaVacia()
        {
            const int CERO = 0;
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<string> listaNombres = new List<string>
            {
                "archivoInexistente1",
                "archivoInexistente2",
                "archivoInexistente3"
            };
            List<string> resultado = mediador.ObtenerRutasPorNombreArchivo(listaNombres);
            Assert.AreEqual(CERO, resultado.Count, "Se esperaba una lista vacía cuando ninguno de los archivos existe.");
        }

        [TestMethod]
        public void ObtenerRutasPorNombreArchivo_ListaNombresVacia_RetornaListaVacia()
        {
            const int CERO =0;
            var tematica = TematicaPartida.Mitologia;
            var mediador = new MediadorPartida(tematica);
            List<string> listaNombres = new List<string>();
            List<string> resultado = mediador.ObtenerRutasPorNombreArchivo(listaNombres);
            Assert.AreEqual(CERO, resultado.Count, "Se esperaba una lista vacía cuando la lista de nombres proporcionada está vacía.");
        }

        #endregion
    }
}
