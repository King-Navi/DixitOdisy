using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Pruebas.DAO.Utilidades
{
    public abstract class ConfiguracionPruebaBD
    {
        [TestInitialize]
        public virtual void ConfigurarPruebas()
        {
            Dictionary<string, object> resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            resultado.TryGetValue(Llaves.LLAVE_MENSAJE, out object mensaje);
            Console.WriteLine((string)mensaje);
            resultado.TryGetValue(Llaves.LLAVE_ERROR, out object fueExitoso);
            if ((bool)fueExitoso)
            {
                Assert.Fail("La BD no está configurada.");
            }
        }
    }
}
