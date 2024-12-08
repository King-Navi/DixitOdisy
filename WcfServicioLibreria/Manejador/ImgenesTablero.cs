using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioImagenesTablero
    {
        public async Task<bool> MostrarImagenesTablero(string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            try
            {
                var contexto = contextoOperacion.GetCallbackChannel<IImagenesTableroCallback>();
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                if (partida != null)
                {
                    var listaCalvesImagenesTablero = partida.ImagenesTablero.Values.SelectMany(lista => lista).ToList();
                    IMediadorImagen mediador = partida.mediadorImagen;
                    ManejadorImagen manejadorImagen = new ManejadorImagen(
                        Escritor,
                        mediador,
                        partida.tematica,
                        new LectorDisco(),
                       listaCalvesImagenesTablero);
                    return await manejadorImagen.SolicitarTableroCartasAsync(contexto);
                }
                return true;
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionError(excepcion);
            }
            return false;
        }
    }
}
