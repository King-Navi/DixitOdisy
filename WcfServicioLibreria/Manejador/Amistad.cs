using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioAmistad
    {
        public int ActualizarAmigo(string nombreRemitente, string nombreDestinatario, string peticionEstado)
        {
            throw new NotImplementedException();
        }

        public void AgregarAmigo(Usuario remitente, string destinatario) //FIXME
        {
            //TODO:Se necesita un metodo que convierta de gamertag a id de base de datos
            //ObtenerIdPorNombre(remitente.Nombre) En esta caso para prueba sera 2
            int idRemitente = 2;
            //ObtenerIdPorNombre(destinatario.Nombre) En esta caso para prueba sera 1
            int idDestinatario = 1;

            //TODO:Validar que no son amigos en la base de datos (Nota para la busqueda el primer id debe ser el alto)
            //ValidacionNoAmistad(id, id);
            if (false)
            {
                throw new FaultException<AmistadFalla>(new AmistadFalla(true, false));
            }

            var idMasAlto = Math.Max(idDestinatario, idRemitente);
            var idMasBajo = Math.Min(idDestinatario, idRemitente);
            //ExisteSolicitudAmistad(idMasAlto,idMasBajo)   Consulta a BD, colocar resultado en if
            if (false)
            {
                throw new FaultException<AmistadFalla>(new AmistadFalla(false, true));
            }

            if (jugadoresConectadosDiccionario.ContainsKey(idDestinatario))
            {
                EnviarSolicitudJugadorConectado(remitente, idDestinatario);
            }
        }
        private void EnviarSolicitudJugadorConectado(Usuario remitente, int idDestinatario) 
        {
            jugadoresConectadosDiccionario.TryGetValue(idDestinatario, out UsuarioContexto destinatarioConectado);
            lock (destinatarioConectado)
            {
                ((IUsuarioAmistad)destinatarioConectado).PeticionAmistadCallBack.ObtenerPeticionAmistadCallback(
                    new SolicitudAmistad()
                    {
                        Remitente = remitente
                    }
                );
            }
        }


        public int BorrarAmigo(Amigo amigo)
        {
            throw new NotImplementedException();
        }
    }
}
