﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using UtilidadesLibreria;
using WpfCliente.Interfaz;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEspera : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
        public SalaEspera(string idSala)
        {
            CambiarIdioma.LenguajeCambiado += LenguajeCambiadoManejadorEvento;
            InitializeComponent();
            if (idSala == null)
            {
                GenerarSalaComoAnfitrion();
            }
            else
            {
                Singleton.Instance.IdSala = idSala;
                Singleton.Instance.IdChat = idSala;
                UnirseSala(idSala);
            }
        }
        private void UnirseSala(string idSala)
        {
            if (!Validacion.ExisteSala(idSala))
            {
                //No existe la sala ¿¿??
                this.Close();
                return;
            }
            if (!Conexion.AbrirConexionSalaJugadorClienteCallback(this))
            {
                //NoHayConexion()
                this.Close();
                return;
            }
            Conexion.SalaJugadorCliente.AgregarJugadorSala(Singleton.Instance.NombreUsuario, idSala);
            labelCodigoSala.Content += idSala;
            UnirseChat();

        }

        private void UnirseChat()
        {
            Conexion.AbrirConexionChatMotorCallback(chatUserControl);
            Conexion.ChatMotorCliente.AgregarUsuarioChat(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
            
        }

        private void GenerarSalaComoAnfitrion()
        {
            try
            {
                var manejadorServicio = new ServicioManejador<ServicioSalaClient>();
                Singleton.Instance.IdSala = manejadorServicio.EjecutarServicio(proxy =>
                 {
                     return proxy.CrearSala(Singleton.Instance.NombreUsuario);
                 });
                Singleton.Instance.IdChat = Singleton.Instance.IdSala;

            }
            catch (Exception excepcion)
            {
                //TODO: Manejo de otras excepciones
                NoHayConexion();
            }
            CrearChat();
            UnirseSala(Singleton.Instance.IdSala);

        }

        private void CrearChat()
        {
            //TODO: Agregar caso en el que no hay conexion
            try
            {
                var manajadorServicio = new ServicioManejador<ServicioChatClient>();
                bool resultado = manajadorServicio.EjecutarServicio(proxy =>
                {
                    return proxy.CrearChat(Singleton.Instance.IdChat);
                });
            }
            catch (Exception)
            {
                //TODO: Manejo de otras excepciones
                NoHayConexion();
            }

        }
        private void NoHayConexion()
        {
            throw new NotImplementedException();
        }
        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            //TODO:Actualizar toda la inferfaz, Pedir a Unnay los .resx
        }

        public void ObtenerJugadoresSalaCallback(string[] jugardoresEnSala)
        {
            throw new NotImplementedException();
        }

        public void EmpezarPartidaCallBack()
        {
            throw new NotImplementedException();
        }

        public void AsignarColorCallback(Dictionary<string, char> jugadoresColores)
        {
            throw new NotImplementedException();
        }

        private void CerrandoVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CambiarIdioma.LenguajeCambiado -= LenguajeCambiadoManejadorEvento;
            try
            {
                if (Conexion.ChatMotorCliente != null)
                {
                    Conexion.ChatMotorCliente.Close();
                    Conexion.ChatMotorCliente = null;
                }
                if (Conexion.SalaJugadorCliente != null)
                {
                    Conexion.SalaJugadorCliente.Close();
                    Conexion.SalaJugadorCliente = null;

                }
            }
            catch (Exception excepcion)
            {

                throw;
            }

        }
    }
}