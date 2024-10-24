using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioPartida
    {


        public string ComenzarPartidaAnfrition(string nombre, ConfiguracionPartida configuracion)
        {
            string resultado = null;
            try
            {
                IPartidaCallabck usuarioSolicitante = contextoOperacion.GetCallbackChannel<IPartidaCallabck>();
                string idPartida = Utilidad.GenerarIdUnico();
                Partida partida = new Partida(idPartida, nombre, configuracion);
                //TODO realmnete aqui deberia ir configuraciones de partida
                partidasdDiccionario.TryAdd(idPartida, partida);
                resultado = idPartida;
            }
            catch (Exception excepcion)
            {

            };

            return resultado;
        }
        public void AvanzarRonda()
        {
            throw new NotImplementedException();
        }
        public void UniserPartida(string idPartida)
        {
            throw new NotImplementedException();
        }

        public void ConfirmarMovimiento()
        {
            throw new NotImplementedException();
        }

        public void EnviarImagenCarta()
        {
            throw new NotImplementedException();
        }

        public void ExpulsarJugador()
        {
            throw new NotImplementedException();
        }

        public void FinalizarPartida()
        {
            throw new NotImplementedException();
        }

        
    }
}
