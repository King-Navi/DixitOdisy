using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pruebas.DAO.Utilidades
{
    public abstract class ConfiguracionPruebaBDInvalida
    {
        [TestInitialize]
        public void ConfigurarConexionInvalida()
        {
            ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "BaseDeDatosInvalida", "usuario", "contraseñaIncorrecta");
        }
    }
}
