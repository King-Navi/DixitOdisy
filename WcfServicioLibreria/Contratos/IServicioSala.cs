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
        void BorrarSala(object sender, EventArgs e);
        [OperationContract]
        bool ValidarSala(string idSala);
    }
}
