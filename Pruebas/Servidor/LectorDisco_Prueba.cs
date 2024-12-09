using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Enumerador;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor
{
    [TestClass]
    public class LectorDisco_Prueba 
    {
        MediadorPartida mediador;
        String ruta;
        [TestInitialize]
        public void ConfigurarRuta()
        {
            ruta = Rutas.CalcularRutaImagenes(TematicaPartida.Mixta);
            mediador= new MediadorPartida(TematicaPartida.Mixta);
        }

        [TestCleanup]
        public void Limpiar()
        {
            ruta = null;
            mediador = null;
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task LeerArchivoAsync_ArchivoNoExiste_LanzaExcepcion()
        {
            var lectorDisco = new LectorDisco();
            string rutaInvalida = "archivo_no_existe.jpg";
            await lectorDisco.LeerArchivoAsync(rutaInvalida);
        }

        [TestMethod]
        public async Task LeerArchivoAsync_ArchivoExiste_RegresarArregloByte()
        {
            var lectorDisco = new LectorDisco();
            mediador = new MediadorPartida(TematicaPartida.Espacio);
            (string RutaCompleta, string NombreArchivo) ruta = mediador.ObtenerRutaCompeltaYNombreImagen();
            var resultado = await lectorDisco.LeerArchivoAsync(ruta.RutaCompleta);
            Assert.IsNotNull(resultado);
        }

    }
}
