﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public class SalaCallbackImpl : ICommunicationObjectImpl, ISalaJugadorCallback
    {
        public ObservableCollection<Usuario> JugadoresEnSala { get; set; } = new ObservableCollection<Usuario>();


        public void EliminarJugadorSalaCallback(Usuario jugardoreRetiradoDeSala)
        {
            var usuarioAEliminar = JugadoresEnSala.FirstOrDefault(busqueda => busqueda.Nombre == jugardoreRetiradoDeSala.Nombre);
            if (usuarioAEliminar != null)
            {
                JugadoresEnSala.Remove(usuarioAEliminar);
            }
        }

        public void EmpezarPartidaCallBack(string idPartida)
        {
            throw new NotImplementedException();
        }

        public void ObtenerJugadorSalaCallback(Usuario jugardoreNuevoEnSala)
        {
            JugadoresEnSala.Add(jugardoreNuevoEnSala);
        }
    }
}
