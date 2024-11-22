using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioCorreo
    {
        [OperationContract]
        bool VerificarCorreo(Usuario usuario);
        string GenerarCodigo();
        //void EnviarCorreo(string codigo, string correo);
        [OperationContract]
        bool VerificarCodigo(string codigo);
        [OperationContract]
        bool VerificarCorreoConGamertag(Usuario usuario);

    }
}
