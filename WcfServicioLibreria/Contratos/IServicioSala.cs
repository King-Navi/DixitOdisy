using System;
using System.ServiceModel;
using WcfServicioLibreria.Evento;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioSala
    {
        [OperationContract]
        string CrearSala(string nombreUsuarioAnfitrion);
        [OperationContract]
        bool ValidarSala(string idSala);
    }
}
