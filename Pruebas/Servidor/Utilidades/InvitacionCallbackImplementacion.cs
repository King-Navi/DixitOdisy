using System;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public partial class UsuarioSesionCallbackImplementacion : IUsuarioSesionCallback
    {
        public bool InvitacionEnviada { get; private set; }
        public void RecibirInvitacionCallback(InvitacionPartida invitacion)
        {
            InvitacionEnviada = true;
        }
    }
}
