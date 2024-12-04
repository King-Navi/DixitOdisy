using System;
using WpfCliente.Utilidad;

namespace WpfCliente.ImplementacionesCallbacks
{
    public partial class SingletonCanal
    {
        private static readonly Lazy<SingletonCanal> instancia = new Lazy<SingletonCanal>(() => new SingletonCanal());
        public static SingletonCanal Instancia => instancia.Value;
        private SingletonCanal() { }

        public bool AbrirTodaConexion()
        {
            try
            {
                AbrirConexionAmistad();
                AbrirConexionUsuarioSesion();
                AbrirConexionInvitacionParitda();
                LimpiarRecursos();
                return true;
            }
            catch (Exception excepcion)
            {
                CerrarTodaConexion();
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
                return false;
            }
        }

        public void CerrarTodaConexion()
        {
            CerrarConexionInvitacion();
            CerrarConexionUsuarioSesion();
            CerrarConexionUsuarioSesion();
        }

        public void LimpiarRecursos()
        {
            LimpiarRecursosAmigos();
        }
    }
}
