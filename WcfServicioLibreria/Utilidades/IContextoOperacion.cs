namespace WcfServicioLibreria.Utilidades
{
    public interface IContextoOperacion
    {
        T GetCallbackChannel<T>();
    }
}
