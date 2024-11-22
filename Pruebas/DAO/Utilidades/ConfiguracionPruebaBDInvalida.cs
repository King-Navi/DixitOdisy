using DAOLibreria;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
