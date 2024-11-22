using System;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioPartida
    {
        [OperationContract]
        string CrearPartida(string anfitrion, ConfiguracionPartida configuracion);
        void BorrarPartida(object sender, EventArgs e);
        [OperationContract]
        bool ValidarPartida(string idPartida);
        [OperationContract]
        bool EsPartidaEmpezada(string idPartida);
    }

}
