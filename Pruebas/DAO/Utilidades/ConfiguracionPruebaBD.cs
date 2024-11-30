using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Pruebas.DAO.Utilidades
{
    public abstract class ConfiguracionPruebaBD
    {
        public const int ID_INVALIDO = -1;
        public const int ID_INEXISTENTE = 9999;
        public const int ID_VALIDO = 1;

        [TestInitialize]
        public virtual void ConfigurarPruebas()
        {
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            if (!resultado)
            {
                Assert.Fail("La BD no está configurada.");
            }
        }
    }
}
