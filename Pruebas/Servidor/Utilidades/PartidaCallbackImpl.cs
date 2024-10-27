using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

public class PartidaCallbackImpl : ICommunicationObjectImpl, IPartidaCallback
{
    // Variables de prueba para almacenar el estado actual de la partida
    public int RondaActual { get; private set; }
    public bool TurnoPerdido { get; private set; }
    public ImagenCarta UltimaImagenRecibida { get; private set; }
    public ConcurrentDictionary<string, ImagenCarta> Imagenes { get; private set; } = new ConcurrentDictionary<string, ImagenCarta>();
    public bool PartidaFinalizada { get; private set; }
    public ConcurrentDictionary<string, Usuario> JugadoresEnSala { get; private set; } = new ConcurrentDictionary<string, Usuario>();

    // Eventos para facilitar las pruebas y monitorear las llamadas a los métodos
    public event Action<int> OnAvanzarRonda;
    public event Action OnTurnoPerdido;
    public event Action<ImagenCarta> OnRecibirImagen;
    public event Action OnFinalizarPartida;
    public event Action<Usuario> OnObtenerJugadorSala;
    public event Action<Usuario> OnEliminarJugadorSala;

    // Constructor
    public PartidaCallbackImpl()
    {
        RondaActual = 0;
        TurnoPerdido = false;
        PartidaFinalizada = false;
        UltimaImagenRecibida = null;
    }

    // Implementación de IPartidaCallback

    public void AvanzarRondaCallback(int rondaActual)
    {
        RondaActual = rondaActual;
        OnAvanzarRonda?.Invoke(RondaActual);
        Console.WriteLine($"Ronda actualizada a {RondaActual}");
    }

    public void TurnoPerdidoCallback()
    {
        TurnoPerdido = true;
        OnTurnoPerdido?.Invoke();
        Console.WriteLine("Turno perdido");
    }

    public void RecibirImagenCallback(ImagenCarta imagen)
    {
        if (UltimaImagenRecibida == null)
        {
            UltimaImagenRecibida = imagen;
        }
        Imagenes.TryAdd(imagen.IdImagen, imagen);
        OnRecibirImagen?.Invoke(imagen);
        Console.WriteLine("Imagen recibida en callback");
    }

    public void FinalizarPartida()
    {
        PartidaFinalizada = true;
        OnFinalizarPartida?.Invoke();
        Console.WriteLine("Partida finalizada");
    }

    public void ObtenerJugadorPartidaCallback(Usuario jugadorNuevoEnSala)
    {
        JugadoresEnSala.AddOrUpdate(jugadorNuevoEnSala.Nombre, jugadorNuevoEnSala , (key , oldValue) => jugadorNuevoEnSala);
        OnObtenerJugadorSala?.Invoke(jugadorNuevoEnSala);
        Console.WriteLine($"Jugador añadido a la sala: {jugadorNuevoEnSala.Nombre}");
    }

    public void EliminarJugadorPartidaCallback(Usuario jugadorRetiradoDeSala)
    {
        JugadoresEnSala.TryRemove(jugadorRetiradoDeSala.Nombre, out _);
        OnEliminarJugadorSala?.Invoke(jugadorRetiradoDeSala);
        Console.WriteLine($"Jugador eliminado de la sala: {jugadorRetiradoDeSala.Nombre}");
    }
}
