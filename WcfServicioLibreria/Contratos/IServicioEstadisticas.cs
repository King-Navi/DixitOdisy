﻿using DAOLibreria.ModeloBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract]
    public interface IServicioEstadisticas
    {
        [OperationContract]
        Estadistica ObtenerEstadisitca(int idUsuario);
    }
}
