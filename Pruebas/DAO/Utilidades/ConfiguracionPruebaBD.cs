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
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            if (resultado)
            {
                Assert.Fail("La BD no está configurada.");
            }
        }
    }
}
