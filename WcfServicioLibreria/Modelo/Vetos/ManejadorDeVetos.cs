using DAOLibreria.ModeloBD;
using DAOLibreria.DAO;
using System;
using System.Threading.Tasks;
using DAOLibreria.Interfaces;

namespace WcfServicioLibreria.Modelo.Vetos
{
    public class ManejadorDeVetos : IManejadorVeto
    {
        private const int ID_INVALIDO = 0;
        private const string NOMBRE_RESERVADO = "guest";
        private const int DIAS_PRIMER_VETO = 30;
        private readonly IVetoDAO vetoDAO;
        private readonly IUsuarioDAO usuarioDAO;
        private readonly IUsuarioCuentaDAO usuarioCuentaDAO;
        private readonly IExpulsionDAO expulsionDAO;
        public ManejadorDeVetos() 
        {
            vetoDAO = new VetoDAO();
            usuarioDAO = new UsuarioDAO();
            usuarioCuentaDAO = new UsuarioCuentaDAO();
            expulsionDAO = new ExpulsionDAO();
        }
        public ManejadorDeVetos(IVetoDAO _vetoDAO, IUsuarioDAO _usuarioDAO, IUsuarioCuentaDAO _usuarioCuentaDAO, IExpulsionDAO _expulsionDAO) 
        {
            vetoDAO = _vetoDAO;
            usuarioDAO = _usuarioDAO;
            usuarioCuentaDAO = _usuarioCuentaDAO;
            expulsionDAO = _expulsionDAO;
        }

        public async Task<bool> VetaJugadorAsync(string nombreJugador)
        {
            if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
            {
                return false;
            }
            var usuarioModeloBaseDatos = usuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            if (usuarioModeloBaseDatos ==null)
            {
                return false;
            }
            return await BuscarJugadorVetoAsync(usuarioModeloBaseDatos);
        }
        public async Task<bool> RegistrarExpulsionJugadorAsync(string nombreJugador, string motivo, bool esHacker)
        {
            if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
            {
                return false;
            }
            var usuarioModeloBaseDatos = usuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            if (usuarioModeloBaseDatos == null)
            {
                return false;
            }
            return await BuscarJugadorRegistrarExpulsionAsync(usuarioModeloBaseDatos, motivo, esHacker);
        }

        private async Task<bool> BuscarJugadorVetoAsync(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos)
        {
            var idUsuarioCuenta = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
            if (idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            
            if (vetoDAO.ExisteTablaVetoPorIdCuenta((int)idUsuarioCuenta))
            {
                 return CrearRegistroVeto((int)idUsuarioCuenta, DateTime.Now.AddDays(DIAS_PRIMER_VETO), true);
            }
            else if (await Conexion.VerificarConexionAsync())
            {
                return CrearRegistroVeto((int)idUsuarioCuenta, null, true);
            }
            else
            {
                return false;
            }

        }
        private async Task<bool> BuscarJugadorRegistrarExpulsionAsync(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos , string motivo, bool esHacker)
        {
            var idUsuarioCuenta = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
            if (idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            if (!await Conexion.VerificarConexionAsync())
            {
                return false;
            }
            if (!expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar((int)idUsuarioCuenta))
            {
                return expulsionDAO.CrearRegistroExpulsion((int)idUsuarioCuenta, motivo, esHacker);
            }
            else
            {
                expulsionDAO.CambiarExpulsionesAFueronPenalizadas((int)idUsuarioCuenta);
                return await VetaJugadorAsync(usuarioModeloBaseDatos.gamertag);
            }
        }

        private bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente)
        {
            return vetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);
        }
    }
}
