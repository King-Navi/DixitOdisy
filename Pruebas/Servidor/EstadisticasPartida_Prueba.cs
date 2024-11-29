using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.Servidor
{
    [TestClass]
    public class EstadisticasPartida_Prueba : ConfiguradorPruebaParaServicio
    {
        [TestInitialize]
        protected override void ConfigurarManejador()
        {
            base.ConfigurarManejador();
        }
        [TestCleanup]
        protected override void LimpiadorDAOs()
        {
            base.LimpiadorDAOs();
        }
    }
}
