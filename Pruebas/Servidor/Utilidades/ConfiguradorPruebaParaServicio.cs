﻿using DAOLibreria.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Manejador;
using WcfServicioLibreria.Utilidades;

namespace Pruebas.Servidor.Utilidades
{
    public abstract class ConfiguradorPruebaParaServicio
    {
        public const int ID_INVALIDO = -1;
        public const int ID_INEXISTENTE = 9999;
        public const int ID_VALIDO = 1;
        public const string ESTADO_SOLICITUD_PENDIENTE = "Pendiente";
        public int idAleatorioValido;
        protected Mock<IVetoDAO> imitarVetoDAO = new Mock<IVetoDAO>();
        protected Mock<IUsuarioDAO> imitarUsuarioDAO = new Mock<IUsuarioDAO>();
        protected Mock<IUsuarioCuentaDAO> imitarUsuarioCuentaDAO = new Mock<IUsuarioCuentaDAO>();
        protected Mock<IPeticionAmistadDAO> imitarPeticionAmistadDAO = new Mock<IPeticionAmistadDAO>();
        protected Mock<IExpulsionDAO> imitarExpulsionDAO = new Mock<IExpulsionDAO>();
        protected Mock<IEstadisticasDAO> imitarEstadisticasDAO = new Mock<IEstadisticasDAO>();
        protected Mock<IAmistadDAO> imitarAmistadDAO = new Mock<IAmistadDAO>();
        protected Mock<IContextoOperacion> mockContextoProvedor = new Mock<IContextoOperacion>();


        protected ManejadorPrincipal manejador;


        protected virtual void ConfigurarManejador()
        {
            manejador = new ManejadorPrincipal(
                mockContextoProvedor.Object,
                imitarVetoDAO.Object,
                imitarUsuarioDAO.Object,
                imitarUsuarioCuentaDAO.Object,
                imitarPeticionAmistadDAO.Object,
                imitarExpulsionDAO.Object,
                imitarEstadisticasDAO.Object,
                imitarAmistadDAO.Object
            );
            idAleatorioValido = GeneradorAleatorio.GenerarIdValido();
        }
        protected void ConfigurarImitadores()
        {
            imitarPeticionAmistadDAO
                .Setup(dao => dao.AceptarSolicitudAmistad(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int idRemitente, int idDestinatario) =>
                {
                    if (idRemitente == idDestinatario)
                    {
                        return false; 
                    }
                    if (idRemitente <= 0 || idDestinatario <= 0)
                    {
                        return false;
                    }
                    return true;
                });
        }

        protected virtual void LimpiadorTodo()
        {
            manejador = null;
            imitarVetoDAO = null;
            imitarUsuarioDAO = null;
            imitarUsuarioCuentaDAO = null;
            imitarPeticionAmistadDAO = null;
            imitarExpulsionDAO = null;
            imitarEstadisticasDAO = null;
            imitarAmistadDAO = null;
        }

        protected virtual void LimpiadorDAOs()
        {
            imitarVetoDAO = null;
            imitarUsuarioDAO = null;
            imitarUsuarioCuentaDAO = null;
            imitarPeticionAmistadDAO = null;
            imitarExpulsionDAO = null;
            imitarEstadisticasDAO = null;
            imitarAmistadDAO = null;
        }
    }
}