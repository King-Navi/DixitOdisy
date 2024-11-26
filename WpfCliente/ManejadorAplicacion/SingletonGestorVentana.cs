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

namespace WpfCliente.Contexto
{
    public class SingletonGestorVentana
    {
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
            manejadoresEventos.TryAdd(Ventana.IniciarSesion, new VentanaEventoManejador(ventanaPrinciapl));
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
                    return true;
                }
                catch (Exception excepcion)
                {
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
                }
                manejadoresEventos.TryRemove(nombre, out _);
            }
            return false;
        }
        public bool MostrarVentana(Ventana nombre)
        {
            try
            {
                if (manejadoresEventos.ContainsKey(nombre))
                {
                    manejadoresEventos.TryGetValue(nombre, out VentanaEventoManejador ventanaBuscada);
                    ventanaBuscada.Ventana?.Show();
                    return true;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception  excepcion)
            {
                manejadoresEventos.TryRemove(nombre, out _);
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            return false;
        }

        public bool OcultarVentana(Ventana nombre)
        {
            try
            {
                if (manejadoresEventos.ContainsKey(nombre))
                {
                    manejadoresEventos.TryGetValue(nombre, out VentanaEventoManejador ventanaBuscada);
                    ventanaBuscada.Ventana?.Hide();
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
                ManejadorExcepciones.ManejarComponenteErrorExcepcion(excepcion);
            }
            return false;
        }

        public bool AbrirNuevaVentana(Ventana nombre, Window ventana)
        {
            try
            {
                EvaluarVentanaNulo(ventana);
                if (manejadoresEventos.TryAdd(nombre, new VentanaEventoManejador(ventana)))
                {
                    manejadoresEventos.TryGetValue(nombre, out VentanaEventoManejador ventanaBuscada);
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
                ManejadorExcepciones.ManejarComponenteFatalExcepcion(excepcion);
            }
            return false;
        }

        public bool AbrirNuevaVentanaConVuelta(Ventana nombreNueva, Window ventanaNueva, Ventana nombreRegreso)
        {
            try
            {
                EvaluarVentanaNulo(ventanaNueva);
                if (manejadoresEventos.TryAdd(nombreNueva, new VentanaEventoManejador(ventanaNueva) ))
                {
                    SuscribirsaCierre(ventanaNueva, nombreRegreso);
                    manejadoresEventos.TryGetValue(nombreNueva, out VentanaEventoManejador ventanaBuscada);
                    ventanaBuscada.Ventana?.Show();
                    OcultarVentana(nombreRegreso);
                    return true;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception excepcion)
            {
                MostrarVentana(nombreRegreso);
                manejadoresEventos.TryRemove(nombreNueva, out _);
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

        private void EvaluarCierresConexion(Window ventana)
        {
            if (ventana is SalaEsperaWindow)
            {
                Conexion.CerrarChatMotor();
                Conexion.CerrarSalaJugador();
            }
            if (ventana is PartidaWindow)
            {
                Conexion.CerrarPartida();
                Conexion.CerrarChatMotor();
            }
            if (ventana is MenuWindow)
            {
                CerrarTodaConexion();
            }
        }

        private void CerrarTodaConexion()
        {
            SingletonUsuarioSessionJugador.Instancia.CerrarUsuarioSesion();
            Conexion.CerrarPartida();
            Conexion.CerrarChatMotor();
            Conexion.CerrarAmigos();
            Conexion.CerrarConexionInvitacionesPartida();
            Conexion.CerrarSalaJugador();
        }

        private void SuscribirsaCierre(Window ventanaNueva, Ventana ventanaRegreso)
        {
            ventanaNueva.Closed += (objecto, argumentoEvento) =>
            {
                if (!MostrarVentana(ventanaRegreso))
                {
                    AbrirNuevaVentana(Ventana.IniciarSesion, new IniciarSesion());
                    EvaluarCierresConexion(ventanaNueva);
                }
            };
        }
    }
}
