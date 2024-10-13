using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPeticionAmistad
    {
        public void ObtenerPeticionAmistad(string nombreUsuario)
        {
            throw new NotImplementedException();
        }
        public bool AbrirCanalParaPeticiones(Usuario usuario) //FIXME: NO cubre todos los casos (1.- No hay conexion a SQLServer,2.- No existe conexion con los usuario)
        {
            bool existeJugador = jugadoresConectadosDiccionario.TryGetValue(usuario.IdUsuario , out UsuarioContexto usuarioActual);
            if (existeJugador)
            {
                throw new FaultException<UsuarioFalla>(new UsuarioFalla() { ExisteUsuario = false });
            }
            lock (usuarioActual)
            {
                ((IUsuarioAmistad)usuarioActual).PeticionAmistadCallBack = OperationContext.Current.GetCallbackChannel<IServicioPeticionAmistadCallBack>();
                //TODO: SI hya mas cosas que hacer despues de abrir el canal poenr aqui
            }
            return existeJugador;
        }
    }
}
