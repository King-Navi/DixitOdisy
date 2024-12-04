using DAOLibreria.Interfaces;
using Moq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor.Utilidades
{
    public abstract class ConfiguradorPruebaParaServicio
    {
        public const int ID_INVALIDO = -1;
        public const int ID_INEXISTENTE = 9999;
        public const int ID_VALIDO = 1;
        public const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";
        public const string PALABRA_PROHIBIDA_GUEST = "guest";
        public int idAleatorioValido;
        protected Mock<IVetoDAO> imitarVetoDAO = new Mock<IVetoDAO>();
        protected Mock<IUsuarioDAO> imitarUsuarioDAO = new Mock<IUsuarioDAO>();
        protected Mock<IUsuarioCuentaDAO> imitarUsuarioCuentaDAO = new Mock<IUsuarioCuentaDAO>();
        protected Mock<ISolicitudAmistadDAO> imitarPeticionAmistadDAO = new Mock<ISolicitudAmistadDAO>();
        protected Mock<IExpulsionDAO> imitarExpulsionDAO = new Mock<IExpulsionDAO>();
        protected Mock<IEstadisticasDAO> imitarEstadisticasDAO = new Mock<IEstadisticasDAO>();
        protected Mock<IAmistadDAO> imitarAmistadDAO = new Mock<IAmistadDAO>();
        protected Mock<IContextoOperacion> mockContextoProvedor = new Mock<IContextoOperacion>();
        public UsuarioSesionCallbackImplementacion implementacionCallback;

        protected ManejadorPrincipal manejador ;


        public virtual void ConfigurarManejador()
        {
            manejador = new ManejadorPrincipal(
                mockContextoProvedor.Object,
                imitarVetoDAO.Object,
                imitarUsuarioDAO.Object,
                imitarUsuarioCuentaDAO.Object,
                imitarPeticionAmistadDAO.Object,
                imitarExpulsionDAO.Object,
                imitarEstadisticasDAO.Object,
                imitarAmistadDAO.Object
            );
            idAleatorioValido = GeneradorAleatorio.GenerarIdValido();
            implementacionCallback = new Utilidades.UsuarioSesionCallbackImplementacion();
            mockContextoProvedor.Setup(contextoProveedor => contextoProveedor.GetCallbackChannel<IUsuarioSesionCallback>())
                .Returns(implementacionCallback);
        }
        public void ConfigurarImitadores()
        {
            imitarPeticionAmistadDAO
                .Setup(dao => dao.AceptarSolicitudAmistad(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int idRemitente, int idDestinatario) =>
                {
                    if (idRemitente == idDestinatario)
                    {
                        return false; 
                    }
                    if (idRemitente <= 0 || idDestinatario <= 0)
                    {
                        return false;
                    }
                    return true;
                });
        }

        public virtual void LimpiadorTodo()
        {
            manejador = null;
            imitarVetoDAO = null;
            imitarUsuarioDAO = null;
            imitarUsuarioCuentaDAO = null;
            imitarPeticionAmistadDAO = null;
            imitarExpulsionDAO = null;
            imitarEstadisticasDAO = null;
            imitarAmistadDAO = null;
        }

        public virtual void LimpiadorDAOs()
        {
            imitarVetoDAO = null;
            imitarUsuarioDAO = null;
            imitarUsuarioCuentaDAO = null;
            imitarPeticionAmistadDAO = null;
            imitarExpulsionDAO = null;
            imitarEstadisticasDAO = null;
            imitarAmistadDAO = null;
        }
    }
}
