using DAOLibreria.ModeloBD;
using DAOLibreria.DAO;
using System;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo.Vetos
{
    public class ManejadorDeVetos : IManejadorVeto
    {
        private const int ID_INVALIDO = 0;
        private const string NOMBRE_RESERVADO = "guest";
        private const int DIAS_PRIMER_VETO = 30;
        public ManejadorDeVetos() { }

        public async Task<bool> VetaJugadorAsync(string nombreJugador)
        {
            if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
            {
                return false;
            }
            var usuarioModeloBaseDatos = UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            if (usuarioModeloBaseDatos ==null)
            {
                return false;
            }
            return await BuscarJugadorVeto(usuarioModeloBaseDatos);
        }
        public async Task<bool> RegistrarExpulsionJugadorAsync(string nombreJugador, string motivo, bool esHacker)
        {
            if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
            {
                return false;
            }
            var usuarioModeloBaseDatos = UsuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
            if (usuarioModeloBaseDatos == null)
            {
                return false;
            }
            return await BuscarJugadorRegistrarExpulsion(usuarioModeloBaseDatos, motivo, esHacker);
        }

        private async Task<bool> BuscarJugadorVeto(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos)
        {
            var idUsuarioCuenta = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
            if (idUsuarioCuenta == null || idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            
            if (VetoDAO.ExisteTablaVetoPorIdCuenta((int)idUsuarioCuenta))
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
        private async Task<bool> BuscarJugadorRegistrarExpulsion(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos , string motivo, bool esHacker)
        {
            var idUsuarioCuenta = UsuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
            if (idUsuarioCuenta == null || idUsuarioCuenta <= ID_INVALIDO)
            {
                return false;
            }
            if (!await Conexion.VerificarConexionAsync())
            {
                return false;
            }
            if (!ExpulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar((int)idUsuarioCuenta))
            {
                return ExpulsionDAO.CrearRegistroExpulsion((int)idUsuarioCuenta, motivo, esHacker);
            }
            else
            {
                ExpulsionDAO.CambiarExpulsionesAFueronPenalizadas((int)idUsuarioCuenta);
                return await VetaJugadorAsync(usuarioModeloBaseDatos.gamertag);
            }
        }

        private bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente)
        {
            return VetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);
        }
    }
}
