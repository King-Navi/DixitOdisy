using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Enumerador;
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
