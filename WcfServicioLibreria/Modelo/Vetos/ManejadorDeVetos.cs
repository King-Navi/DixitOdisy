using DAOLibreria.ModeloBD;
using DAOLibreria.DAO;
using System;
using System.Threading.Tasks;
using DAOLibreria.Interfaces;
using WcfServicioLibreria.Utilidades;
using System.Data.SqlClient;
using System.Security;

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
        public IConexion Conexion { get; set; }
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
            try
            {
                if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
                {
                    return false;
                }
                var usuarioModeloBaseDatos = usuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
                if (usuarioModeloBaseDatos == null)
                {
                    throw new Exception();
                }
                return await BuscarJugadorVetoAsync(usuarioModeloBaseDatos);
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }
        public async Task<bool> RegistrarExpulsionJugadorAsync(string nombreJugador, string motivo, bool esHacker)
        {
            try
            {
                if (nombreJugador.ToLower().Contains(NOMBRE_RESERVADO))
                {
                    return false;
                }
                var usuarioModeloBaseDatos = usuarioDAO.ObtenerUsuarioPorNombre(nombreJugador);
                if (usuarioModeloBaseDatos == null)
                {
                    throw new Exception();
                }
                return await BuscarJugadorRegistrarExpulsionAsync(usuarioModeloBaseDatos, motivo, esHacker);
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        private async Task<bool> BuscarJugadorVetoAsync(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos)
        {
            try
            {
                var idUsuarioCuenta = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
                if (idUsuarioCuenta <= ID_INVALIDO)
                {
                    throw new Exception();
                }

                if (vetoDAO.ExisteTablaVetoPorIdCuenta(idUsuarioCuenta))
                {
                    return CrearRegistroVeto(idUsuarioCuenta, DateTime.Now.AddDays(DIAS_PRIMER_VETO), true);
                }
                else if (await Conexion.VerificarConexionAsync())
                {
                    return CrearRegistroVeto(idUsuarioCuenta, null, true);
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;

        }
        private async Task<bool> BuscarJugadorRegistrarExpulsionAsync(DAOLibreria.ModeloBD.Usuario usuarioModeloBaseDatos , string motivo, bool esHacker)
        {
            try
            {
                var idUsuarioCuenta = usuarioCuentaDAO.ObtenerIdUsuarioCuentaPorIdUsuario(usuarioModeloBaseDatos.idUsuarioCuenta);
                if (idUsuarioCuenta <= ID_INVALIDO)
                {
                    throw new Exception();
                }
                if (!await Conexion.VerificarConexionAsync())
                {
                    throw new Exception();
                }
                if (!expulsionDAO.TieneMasDeDiezExpulsionesSinPenalizar(idUsuarioCuenta))
                {
                    return expulsionDAO.CrearRegistroExpulsion(idUsuarioCuenta, motivo, esHacker);
                }
                else
                {
                    expulsionDAO.CambiarExpulsionesAFueronPenalizadas(idUsuarioCuenta);
                    return await VetaJugadorAsync(usuarioModeloBaseDatos.gamertag);
                }
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }

        private bool CrearRegistroVeto(int idUsuarioCuenta, DateTime? fechaFin, bool esPermanente)
        {
            try
            {
                return vetoDAO.CrearRegistroVeto(idUsuarioCuenta, fechaFin, esPermanente);
            }
            catch (NullReferenceException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }
    }
}
