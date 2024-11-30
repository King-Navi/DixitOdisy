using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public partial class UsuarioSesionCallbackImplementacion : IUsuarioSesionCallback
    {
        ConcurrentDictionary<string, Amigo> amigos = new ConcurrentDictionary<string, Amigo>();
        public void CambiarEstadoAmigoCallback(Amigo amigo)
        {
            amigos.AddOrUpdate(amigo.Nombre,
                amigo,                  
                (clave, viejoValor) => amigo    
            );

        }

        public void EliminarAmigoCallback(Amigo amigo)
        {
            amigos.TryRemove(amigo.Nombre, out _);
        }

        public void ObtenerAmigoCallback(Amigo amigo)
        {
            amigos.TryAdd(amigo.Nombre, amigo);

        }
    }
}
