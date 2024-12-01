using Pruebas.Servidor.Utilidades;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
public class PartidaCallbackImplementacion : ICommunicationObjectImpl, IPartidaCallback
{
    public int RondaActual { get; private set; }
    public bool TurnoPerdido { get; private set; }
    public ImagenCarta UltimaImagenRecibida { get; private set; }
    public ConcurrentDictionary<string, ImagenCarta> Imagenes { get; private set; } = new ConcurrentDictionary<string, ImagenCarta>();
    public bool PartidaFinalizada { get; private set; }
    public ConcurrentDictionary<string, Usuario> JugadoresEnPartida { get; private set; } = new ConcurrentDictionary<string, Usuario>();
    public bool Ping { get; set; }
    private readonly SemaphoreSlim semaphoreRecibirImagenCallback = new SemaphoreSlim(1, 1);

    public event Action<int> OnAvanzarRonda;
    public event Action OnTurnoPerdido;
    public event Action<ImagenCarta> OnRecibirImagen;
    public event Action OnFinalizarPartida;
    public event Action<Usuario> OnObtenerJugadorSala;
    public event Action<Usuario> OnEliminarJugadorSala;

    public PartidaCallbackImplementacion()
    {
        RondaActual = 0;
        TurnoPerdido = false;
        PartidaFinalizada = false;
        UltimaImagenRecibida = null;
    }


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
        semaphoreRecibirImagenCallback.Wait();
        if (UltimaImagenRecibida == null)
        {
            UltimaImagenRecibida = imagen;
        }
        Imagenes.TryAdd(imagen.IdImagen, imagen);
        OnRecibirImagen?.Invoke(imagen);
        Console.WriteLine("Imagen recibida en callback");
        semaphoreRecibirImagenCallback.Release();
    }

    public void FinalizarPartidaCallback()
    {
        PartidaFinalizada = true;
        OnFinalizarPartida?.Invoke();
        Console.WriteLine("Partida finalizada");
    }

    public void ObtenerJugadorPartidaCallback(Usuario jugadorNuevoEnSala)
    {
        JugadoresEnPartida.AddOrUpdate(jugadorNuevoEnSala.Nombre, jugadorNuevoEnSala , (key , oldValue) => jugadorNuevoEnSala);
        OnObtenerJugadorSala?.Invoke(jugadorNuevoEnSala);
        Console.WriteLine($"Jugador añadido a la sala: {jugadorNuevoEnSala.Nombre}");
    }

    public void EliminarJugadorPartidaCallback(Usuario jugadorRetiradoDeSala)
    {
        JugadoresEnPartida.TryRemove(jugadorRetiradoDeSala.Nombre, out _);
        OnEliminarJugadorSala?.Invoke(jugadorRetiradoDeSala);
        Console.WriteLine($"Jugador eliminado de la sala: {jugadorRetiradoDeSala.Nombre}");
    }

    public void NotificarNarradorCallback(bool esNarrador)
    {
        //TODO:
    }

    public void MostrarPistaCallback(string pista)
    {
        //TODO:
    }

    public void EnviarEstadisticasCallback(EstadisticasPartida estadisticas)
    {
        //TODO: 
    }

    public void CambiarPantallaCallback(int numeroPantalla)
    {
        //TODO: 

    }

    public void IniciarValoresPartidaCallback(bool seUnio)
    {
        //TODO: 

    }

    public void RecibirGrupoImagenCallback(ImagenCarta imagen)
    {
        //TODO: 
    }
}
