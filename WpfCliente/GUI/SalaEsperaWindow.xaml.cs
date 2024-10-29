﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using WpfCliente.Utilidad;

namespace WpfCliente.GUI
{
    /// <summary>
    /// Lógica de interacción para SalaEspera.xaml
    /// </summary>
    public partial class SalaEspera : Window, IActualizacionUI, IServicioSalaJugadorCallback
    {
        public ObservableCollection<Usuario> jugadoresSala = new ObservableCollection<Usuario>();
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
            DataContext = this;
        }
        private void UnirseSala(string idSala)
        {
            //Task<bool> verificarConexion = Validacion.ValidarConexion();
            //HabilitarBotones(false);
            //if (!await verificarConexion)
            //{
            //    VentanasEmergentes.CrearVentanaEmergenteErrorServidor(this);
            //    this.Close();

            //    return;
            //}
            //HabilitarBotones(true);

            if (!Validacion.ExisteSala(idSala))
            {
                //No existe la sala ¿¿??
                this.Close();
                return;
            }
            var resultadoTask = Conexion.AbrirConexionSalaJugadorCallbackAsync(this);
            bool resultado = resultadoTask.Result;

            if (!resultado)
            {
                NoHayConexion();
                return;
            }
            Conexion.SalaJugador.AgregarJugadorSala(Singleton.Instance.NombreUsuario, idSala);
            labelCodigo.Content += idSala;
            UnirseChat();

        }

        private void HabilitarBotones(bool v)
        {
            //TODO
        }

        private void UnirseChat()
        {
            Conexion.AbrirConexionChatMotorCallbackAsync(chatUserControl);
            Conexion.ChatMotor.AgregarUsuarioChat(Singleton.Instance.IdChat, Singleton.Instance.NombreUsuario);
            
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

                CrearChat();
                UnirseSala(Singleton.Instance.IdSala);
            }
            catch (Exception excepcion)
            {
                //TODO: Manejo de otras excepciones
                NoHayConexion();
            }

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
            this.Close();
        }
        public void LenguajeCambiadoManejadorEvento(object sender, EventArgs e)
        {
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            labelCodigoSala.Content = Idioma.labelCodigoSala;
            labelUsuariosLobby.Content = Idioma.labelUsuariosLobby;
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
                Conexion.CerrarChatMotor();
                Conexion.CerrarSalaJugador();
            }
            catch (Exception excepcion)
            {
                //TODO Manejar excepcion
            }

        }

        public void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala)
        {
            List<Amigo> amigos = new List<Amigo>();
            usuariosSalaUserControl.ObtenerUsuarioSala(jugardoreNuevoEnSala, amigos);
        }

        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            usuariosSalaUserControl.EliminarUsuarioSala(jugardoreRetiradoDeSala);
        }

        private void LabelCodigoSala_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (labelCodigo.Content != null)
            {
                Clipboard.SetText(labelCodigo.Content.ToString());

                VentanasEmergentes.CrearVentanaEmergenteCodigoCopiado(this);
            }
        }
    }
}
