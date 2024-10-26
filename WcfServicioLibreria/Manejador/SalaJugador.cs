﻿using System;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioSalaJugador
    {
        public void ComenzarPartidaAnfrition(string nombre, string idSala)
        {
            try
            {
                salasDiccionario.TryGetValue(idSala, out Sala sala);
                lock (sala)
                {
                    sala.AvisarComienzoPatida(nombre, idSala);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            };
        }
        /// <summary>
        /// Agrega a un jugador a una sala ya existente
        /// </summary>
        /// <param name="gamertag"></param>
        /// <param name="idSala"></param>
        public void AgregarJugadorSala(string gamertag, string idSala)
        {
            if (!ValidarSala(idSala))
            {
                return;
            }
            try
            {
                ISalaJugadorCallback contexto = contextoOperacion.GetCallbackChannel<ISalaJugadorCallback>();
                salasDiccionario.TryGetValue(idSala, out Modelo.Sala sala);
                lock (sala)
                {
                    sala.AgregarJugadorSala(gamertag, contexto);
                    sala.AvisarNuevoJugador(gamertag);
                }
            }
            catch (Exception excepcion)
            {
                //TODO: Manejar el error
            }
        }

        public void AsignarColor(string idSala)
        {
            throw new NotImplementedException();
        }

        public void EmpezarPartida(string idSala)
        {
            throw new NotImplementedException();
        }
    }
}
