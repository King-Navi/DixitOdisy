
namespace WpfCliente.ServidorDescribelo
{
    public partial class ChatMensaje
    {
        public override string ToString()
        {
            return $"{HoraFecha.ToLocalTime()} {Nombre} : {Mensaje}";
        }
    }
}
