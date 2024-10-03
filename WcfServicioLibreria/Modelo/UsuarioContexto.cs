﻿using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Evento;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-7.0</ref>
    /// <ref>https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-idisposable</ref>
    [DataContract]
    public abstract class UsuarioContexto
    {
        protected int idUsuarioCuenta;
        protected String nombre;
        private bool desechado = false;
        public IUsuarioSesionCallback UsuarioSesionCallBack { get; set; }
        public EventHandler CerrandoEvento { get; set; }
        public EventHandler CerradoEvento { get; set; }
        public EventHandler FalloEvento { get; set; }
        public EventHandler DesconexionManejadorEvento;
        [DataMember]
        public string Nombre { get => nombre; set => nombre = value; }
        [DataMember]
        public int IdUsuario { get => idUsuarioCuenta; set => idUsuarioCuenta = value; }
        /// <summary>
        /// Avisa a todos los sucriptores que se esta desconectando
        /// </summary>
        public void EnDesconexion()
        {
            DesconexionManejadorEvento?.Invoke(null, new UsuarioDesconectadoEventArgs(nombre, DateTime.Now));
        }
        /// <summary>
        /// Libera recursos que de otra manera no serian liberados
        /// </summary>
        public virtual void Desechar()
        {
            if (desechado)
                return;
            DesuscribirseDeEventos();
            UsuarioSesionCallBack = null;
            desechado = true;

        }
        /// <summary>
        /// Elimina las suscripciones los eventos de ICommunicationObject de la sesion de IUsuarioSesionCallback
        /// </summary>
        private void DesuscribirseDeEventos()
        {
            if (UsuarioSesionCallBack is ICommunicationObject comunicacionObjecto)
            {
                if (CerrandoEvento != null)
                {
                    CerrandoEvento = null;
                }

                if (CerradoEvento != null)
                {
                    comunicacionObjecto.Closed -= CerradoEvento;
                    CerradoEvento = null;
                }

                if (FalloEvento != null)
                {
                    comunicacionObjecto.Faulted -= FalloEvento;
                    FalloEvento = null;
                }
            }
        }

    }
}
