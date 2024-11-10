using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.RightsManagement;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
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
