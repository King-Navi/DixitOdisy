using System;

namespace Pruebas.Cliente.Utilidades
{
    public class UsuarioSesionCallbackImpl : ServidorDescribeloPrueba.IServicioUsuarioSesionCallback
    {
        public bool SesionAbierta { get; private set; }

        public UsuarioSesionCallbackImpl()
        {
            SesionAbierta = false;
        }

        public void ObtenerSessionJugadorCallback(bool sesionAbierta)
        {
            SesionAbierta = sesionAbierta;
        }
    }
}
