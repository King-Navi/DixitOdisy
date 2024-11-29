using DAOLibreria.DAO;
using DAOLibreria.Excepciones;
using DAOLibreria.ModeloBD;
using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Modelo.Excepciones;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioUsuario
    {
        public void DesconectarUsuario(object sender, EventArgs e)
        {
            try
            {
                if (sender is UsuarioContexto && e is UsuarioDesconectadoEventArgs eventoDesconexion)
                {
                    if (jugadoresConectadosDiccionario.ContainsKey(eventoDesconexion.IdUsuario))
                    {
                        jugadoresConectadosDiccionario.TryRemove(eventoDesconexion.IdUsuario, out UsuarioContexto usuarioARemover);
                        lock (usuarioARemover)
                        {
                            if (usuarioARemover != null)
                            {
                                usuarioARemover.EnDesconexion();
                                usuarioARemover.Desechar();
                                ((UsuarioContexto)usuarioARemover).DesconexionEvento += DesconectarUsuario;
                            }
                        }
                        usuarioDAO.ColocarUltimaConexion(eventoDesconexion.IdUsuario);
                    }
                }
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
        }

        public bool EditarContraseniaUsuario(string gamertag, string nuevaContrasenia)
        {
            bool resultado = false;

            if (string.IsNullOrEmpty(gamertag) || string.IsNullOrEmpty(nuevaContrasenia))
            {
                return resultado;
            }

            if (!EsSha256Valido(nuevaContrasenia))
            {
                return resultado;
            }

            try
            {
                resultado = usuarioCuentaDAO.EditarContraseniaPorGamertag(gamertag, nuevaContrasenia);
            }
            catch (CommunicationException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception execepcion)
            {
                ManejadorExcepciones.ManejarErrorException(execepcion);
            }

            return resultado;
        }

        public bool EditarUsuario(Modelo.Usuario usuarioEditado)
        {
            bool resultado = false;
            if (usuarioEditado == null
                || usuarioEditado.IdUsuario <= 0
                || usuarioEditado.Nombre == null)
            {
                return resultado;
            }
            if (usuarioEditado.ContraseniaHASH != null)
            {
                if (!EsSha256Valido(usuarioEditado.ContraseniaHASH))
                    return resultado;
            }
            try
            {
                DAOLibreria.ModeloBD.UsuarioPerfilDTO usuarioPerfilDTO = new UsuarioPerfilDTO()
                {
                    IdUsuario = usuarioEditado.IdUsuario,
                    NombreUsuario = usuarioEditado.Nombre,
                    Correo = usuarioEditado.Correo ?? null,
                    FotoPerfil = usuarioEditado.FotoUsuario != null ? Utilidad.StreamABytes(usuarioEditado.FotoUsuario) : null,
                    HashContrasenia = usuarioEditado.ContraseniaHASH ?? null,
                };
                bool consulta = usuarioDAO.EditarUsuario(usuarioPerfilDTO);
                resultado = true;

            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return resultado;
        }

        public bool Ping()
        {
            return true;
        }

        public async Task<bool> PingBDAsync()
        {
            return await DAOLibreria.ModeloBD.Conexion.VerificarConexionAsync();
        }

        public WcfServicioLibreria.Modelo.Usuario ValidarCredenciales(string gamertag, string contrasenia)
        {
            WcfServicioLibreria.Modelo.Usuario usuario = null;
            try
            {
                DAOLibreria.ModeloBD.UsuarioPerfilDTO usuarioConsulta = usuarioDAO.ValidarCredenciales(gamertag, contrasenia);
                if (usuarioConsulta != null)
                {
                    vetoDAO.VerificarVetoPorIdCuenta(usuarioConsulta.IdUsuarioCuenta);
                    usuario = new WcfServicioLibreria.Modelo.Usuario();
                    if (usuarioConsulta.FotoPerfil != null && usuarioConsulta.FotoPerfil.Length > 0)
                    {
                        usuario.FotoUsuario = new MemoryStream(usuarioConsulta.FotoPerfil, writable: false);
                    }
                    
                    usuario.Nombre = usuarioConsulta.NombreUsuario;
                    usuario.Correo = usuarioConsulta.Correo;
                    usuario.ContraseniaHASH = usuarioConsulta.HashContrasenia;
                    usuario.IdUsuario = usuarioConsulta.IdUsuario;
                }

            }
            catch(VetoEnProgresoExcepcion)
            {
                VetoFalla excepcion = new VetoFalla()
                {
                    EnProgreso = true
                };
                throw new FaultException<VetoFalla>(excepcion, new FaultReason("Veto en progreso"));
            }
            catch (VetoPermanenteExcepcion)
            {
                VetoFalla excepcion = new VetoFalla()
                {
                    EsPermanete = true
                };
                throw new FaultException<VetoFalla>(excepcion, new FaultReason("Veto en permanete, y no vuelvas!!!"));
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }

            return usuario;
        }
        public bool YaIniciadoSesion(string nombreUsuario)
        {
            DAOLibreria.ModeloBD.Usuario usuario = usuarioDAO.ObtenerUsuarioPorNombre(nombreUsuario);
            if (usuario == null)
            {
                return false;
            }
            return jugadoresConectadosDiccionario.ContainsKey(usuario.idUsuario);
        }

        public void JugadoresConectado()
        {
            foreach (UsuarioContexto jugador in jugadoresConectadosDiccionario.Values)
            {
                try
                {
                    Modelo.Usuario usuarioActual = jugador as Modelo.Usuario;
                    Console.WriteLine(usuarioActual.Nombre);

                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarErrorException(excepcion);
                }
            }
        }

        

    }
}
