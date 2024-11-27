using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCliente.GUI;
using WpfCliente.ImplementacionesCallbacks;
using WpfCliente.ManejadorAplicacion;
using WpfCliente.Utilidad;
using WpfCliente.Interfaz;
using WpfCliente.Properties;
using WpfCliente.ServidorDescribelo;
using System.Windows.Input;

namespace WpfCliente.Contexto
{
    public class SingletonGestorVentana : IObservadorVentana
    {
        private const string NOMBRE_INVITADO = "guest";
        private static readonly Lazy<SingletonGestorVentana> instancia =
            new Lazy<SingletonGestorVentana>(() => new SingletonGestorVentana());
        private readonly ConcurrentDictionary<Ventana, VentanaEventoManejador> manejadoresEventos = new ConcurrentDictionary<Ventana, VentanaEventoManejador>();

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
            IniciarSesion ventanaPrinciapl = new IniciarSesion();
            manejadoresEventos.TryAdd(Ventana.IniciarSesion, new VentanaEventoManejador(ventanaPrinciapl, this, Ventana.IniciarSesion));
            ventanaPrinciapl.Show();
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

        

        public void MostrarTodasVentanas()
        {
            if (manejadoresEventos.Keys.Count == 0)
            {
                var inicioSesion = new IniciarSesion();
                AbrirNuevaVentana(Ventana.IniciarSesion, inicioSesion);
                MessageBox.Show("Error no se encontraron mas ventanas se te llevara a inicio");
            }
            Console.WriteLine(manejadoresEventos.Keys.Count);
            Console.WriteLine(manejadoresEventos.Values.Count);
        }

        public void IntentarRegresarMenu()
        {
            if (CierreInvitado())
            {
                return;
            }
            CerrarTodaVentana();
            AbrirNuevaVentana(Ventana.Menu , new MenuWindow());
        }

        public void IntentarRegresarInicio()
        {
            CerrarTodaConexion();
            CerrarTodaVentana();
            AbrirNuevaVentana(Ventana.IniciarSesion, new IniciarSesion());
        }


        public bool AbrirNuevaVentana(Ventana nombre, Window ventana)
        {
            try
            {
                EvaluarVentanaNulo(ventana);
                if (manejadoresEventos.TryAdd(nombre, new VentanaEventoManejador(ventana, this, nombre)))
                {
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
                MostrarTodasVentanas();
                EnCierre(nombre);
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

        private void EvaluarVentanaNulo(Window ventana)
        {
            if (ventana == null)
            {
                throw new ArgumentNullException();
            }
        }

        private void EvaluarCierresConexionAlCerrar(Ventana ventanaCerrada)
        {
            if (Ventana.SalaEspera == ventanaCerrada)
            {
                Conexion.CerrarChatMotor();
                SingletonSalaJugador.Instancia.CerrarConexion();
                CierreInvitado();
                return;
            }
            if (Ventana.Partida== ventanaCerrada)
            {
                Conexion.CerrarPartida();
                Conexion.CerrarChatMotor();
                CierreInvitado();
                return;
            }
            if (Ventana.EditarPerfil== ventanaCerrada)
            {
                return;
            }
            if (manejadoresEventos.ContainsKey(Ventana.IniciarSesion) &&
                manejadoresEventos.Count <= 1)
            {
                CerrarTodaConexion();
                return;
            }
            if (manejadoresEventos.Count <= 0)
            {
                CerrarTodaConexion();
                return;
            }
        }

        private bool CierreInvitado()
        {
            if (!string.IsNullOrEmpty(SingletonCliente.Instance.NombreUsuario) &&
                    SingletonCliente.Instance.NombreUsuario.ToLower().Contains(NOMBRE_INVITADO))
            {
                CerrarTodaConexion();
                CerrarTodaVentana();
                IniciarSesion iniciarSesion = new IniciarSesion();
                AbrirNuevaVentana(Ventana.IniciarSesion,iniciarSesion);
                VentanasEmergentes.CrearVentanaEmergente(Idioma.tituloGraciaPorJugar, Idioma.mensajeConsideraRegistrarteInvitado, iniciarSesion);
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

        public void EnCierre(Ventana nombre)
        {
            manejadoresEventos.TryRemove(nombre, out _);
            EvaluarCierresConexionAlCerrar(nombre);
        }
    }
}
