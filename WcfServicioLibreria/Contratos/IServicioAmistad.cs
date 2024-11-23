﻿using System.Collections.Generic;
using System.ServiceModel;
using WcfServicioLibreria.Modelo;

namespace WcfServicioLibreria.Contratos
{
    [ServiceContract(CallbackContract = typeof(IAmistadCallBack))]
    public interface IServicioAmistad
    {
        [OperationContract(IsOneWay = true)]
        void AbrirCanalParaPeticiones(Usuario usuario);
        [OperationContract]
        [FaultContract(typeof(SolicitudAmistadFalla))]
        bool EnviarSolicitudAmistad (Usuario usuarioRemitente, string destinatario);
        [OperationContract]
        List<SolicitudAmistad> ObtenerSolicitudesAmistad (Usuario usuario);
        [OperationContract]
        bool AceptarSolicitudAmistad(int idRemitente, int idDestinatario);
        [OperationContract]
        bool RechazarSolicitudAmistad(int idRemitente, int idDestinatario);
        [OperationContract]
        bool SonAmigos(string usuarioRemitente, string destinatario);
    }
    [ServiceContract]
    public interface IAmistadCallBack
    {
        [OperationContract]
        void CambiarEstadoAmigo(Amigo amigo);
        [OperationContract]
        void ObtenerAmigoCallback(Amigo amigo);
    }
}
