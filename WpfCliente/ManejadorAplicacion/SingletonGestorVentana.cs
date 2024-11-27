using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using WpfCliente.GUI;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.Interfaz;
using WpfCliente.ManejadorAplicacion;
using WpfCliente.Properties;
using WpfCliente.Utilidad;

namespace WpfCliente.Contexto
{
    public class SingletonGestorVentana : IObservadorVentana
    {
        private const string NOMBRE_INVITADO = "guest";
        private const int NO_HAY_VENTANAS = 0;
        private static readonly Lazy<SingletonGestorVentana> instancia =
            new Lazy<SingletonGestorVentana>(() => new SingletonGestorVentana());
        private readonly ConcurrentDictionary<Ventana, VentanaEventoManejador> manejadoresEventos = new ConcurrentDictionary<Ventana, VentanaEventoManejador>();
        private Ventana ventanaAnterior;
        private readonly Stack<Ventana> ventanaPila = new Stack<Ventana>();   
        public static SingletonGestorVentana Instancia => instancia.Value;
        private SingletonGestorVentana()
        {
        }
        public void Iniciar()
        {
            foreach (var llave in manejadoresEventos.Keys)
            {
                manejadoresEventos.TryGetValue(llave, out var ventanaActual);
                try
                {
                    ventanaActual.Ventana?.Close();
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            }
            IniciarSesionWindow ventanaPrinciapl = new IniciarSesionWindow();
            manejadoresEventos.TryAdd(Ventana.IniciarSesion, new VentanaEventoManejador(ventanaPrinciapl, this, Ventana.IniciarSesion));
            ventanaPrinciapl.Show();
            ventanaAnterior = Ventana.IniciarSesion;
            ventanaPila.Push(ventanaAnterior);
        }

        public bool CerrarVentana(Ventana nombre)
        {

            if (manejadoresEventos.ContainsKey(nombre))
            {
                try
                {
                    manejadoresEventos.TryGetValue(nombre, out VentanaEventoManejador ventanaBuscada);
                    ventanaBuscada.Ventana?.Close();
                }
                catch (Exception excepcion)
                {
                    EnCierre(nombre);
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            }
            return true;
        }

        //public void IntentarRegresarMenu(Ventana ventanaQueL)
        //{
        //    if (CierreInvitado())
        //    {
        //        return;
        //    }
        //    CerrarTodaVentana();
        //    AbrirNuevaVentana(Ventana.Menu, new MenuWindow());
        //}

        //public void IntentarRegresarInicio()
        //{
        //    CerrarTodaConexion();
        //    CerrarTodaVentana();
        //    AbrirNuevaVentana(Ventana.IniciarSesion, new IniciarSesion());
        //}


        public bool AbrirNuevaVentana(Ventana nombre, Window ventana)
        {
            try
            {
                EvaluarVentanaNulo(ventana);
                if (manejadoresEventos.TryAdd(nombre, new VentanaEventoManejador(ventana, this, nombre)))
                {
                    ventanaPila.Push(nombre);
                    manejadoresEventos.TryGetValue(nombre, out VentanaEventoManejador ventanaBuscada);
                    if (nombre == Ventana.IniciarSesion)
                    {
                        CerrarTodaConexion();
                    }
                    ventanaBuscada.Ventana?.Show();
                    return true;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception excepcion)
            {
                manejadoresEventos.TryRemove(nombre, out _);
                EnCierre(nombre);
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }


        private void EvaluarCierresConexionAlCerrar(Ventana ventanaCerrada)
        {
            if (Ventana.SalaEspera == ventanaCerrada)
            {
                Conexion.CerrarChatMotor();
                SingletonSalaJugador.Instancia.CerrarConexion();
                return;
            }
            else if (Ventana.Partida == ventanaCerrada)
            {
                Conexion.CerrarPartida();
                Conexion.CerrarChatMotor();
                return;
            }
            else if (manejadoresEventos.ContainsKey(Ventana.IniciarSesion) &&
                manejadoresEventos.Count <= 1)
            {
                CerrarTodaConexion();
                return;
            }
            else if (manejadoresEventos.Count <= NO_HAY_VENTANAS)
            {
                CerrarTodaConexion();
                return;
            }
        }

        private void EvaluarSiguienteVentanaNoInvitado(Ventana actualCerrada)
        {
            if (Ventana.EditarPerfil == actualCerrada &&
                Ventana.Menu == ventanaAnterior)
            {
                AbrirNuevaVentana(ventanaAnterior, new MenuWindow());
                return;
            }
            if (Ventana.CambiarContrasenia == actualCerrada &&
                Ventana.IniciarSesion == ventanaAnterior)
            {
                AbrirNuevaVentana(ventanaAnterior, new IniciarSesionWindow());
                return;
            }
            if (Ventana.RegistrarUsuario == actualCerrada &&
                Ventana.IniciarSesion == ventanaAnterior)
            {
                AbrirNuevaVentana(ventanaAnterior, new IniciarSesionWindow());
                return;
            }
        }

        private bool CierreInvitado()
        {
            if (!string.IsNullOrEmpty(SingletonCliente.Instance.NombreUsuario) &&
                    SingletonCliente.Instance.NombreUsuario.ToLower().Contains(NOMBRE_INVITADO))
            {
                 return true;
            }
            return false;
        }

        private void CerrarTodaVentana()
        {
            foreach (var ventana in manejadoresEventos)
            {
                manejadoresEventos.TryGetValue(ventana.Key, out VentanaEventoManejador ventanaBuscada);
                try
                {
                    ventanaBuscada.Ventana?.Close();
                }
                catch (Exception excepcion)
                {
                    EnCierre(ventana.Key);
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
            }
        }

        private void CerrarTodaConexion()
        {
            SingletonUsuarioSessionJugador.Instancia.CerrarConexion();
            Conexion.CerrarPartida();
            Conexion.CerrarChatMotor();
            SingletonAmigos.Instancia.CerrarConexion();
            SingletonInvitacionPartida.Instancia.CerrarConexion();
            SingletonSalaJugador.Instancia.CerrarConexion();
        }

        public void EnCierre(Ventana ventanaActualCerrada)
        {
            ventanaAnterior =ventanaPila.Pop();
            manejadoresEventos.TryRemove(ventanaActualCerrada, out _);
            EvaluarCierresConexionAlCerrar(ventanaActualCerrada);
            if (!CierreInvitado())
            {
                EvaluarSiguienteVentanaNoInvitado(ventanaActualCerrada);
            }
            else
            {
                EvaluarSiguienteVentanaInvitado();
            };
        }

        private void EvaluarSiguienteVentanaInvitado()
        {
            if (Ventana.SalaEspera == ventanaAnterior)
            {
                Conexion.CerrarChatMotor();
                SingletonSalaJugador.Instancia.CerrarConexion();
                CerrarTodaConexion();
                CerrarTodaVentana();
                IniciarSesionWindow iniciarSesion = new IniciarSesionWindow();
                AbrirNuevaVentana(Ventana.IniciarSesion, iniciarSesion);
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloGraciaPorJugar, Idioma.mensajeConsideraRegistrarteInvitado, iniciarSesion);
                return;
            }
            else if (Ventana.Partida == ventanaAnterior)
            {
                Conexion.CerrarPartida();
                Conexion.CerrarChatMotor();
                CerrarTodaConexion();
                CerrarTodaVentana();
                IniciarSesionWindow iniciarSesion = new IniciarSesionWindow();
                AbrirNuevaVentana(Ventana.IniciarSesion, iniciarSesion);
                return;
            }
        }

        private void EvaluarVentanaNulo(Window ventana)
        {
            if (ventana == null)
            {
                throw new ArgumentNullException();
            }
        }

    }
}
