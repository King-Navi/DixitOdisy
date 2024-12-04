using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor
{
    [TestClass]
    public class MediadorPartida_Prueba
    {
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
        public void ObtenerArchivosCache_ImagnesDisponibles_DevuelveListaString()
        {
            var tematica = TematicaPartida.Animales;
            var mediador = new MediadorPartida(tematica);
            var lista = mediador.ObtenerArchivosCache();
            foreach (var archivo in lista)
            {
                Console.WriteLine(archivo);



            }
            Assert.IsNotNull(lista, "La ruta devuelta no debe ser nula.");
        }
    }
}
