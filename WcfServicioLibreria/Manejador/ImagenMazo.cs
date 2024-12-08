using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioImagenMazo
    {
        public async Task<bool> SolicitarImagenMazoAsync(string idPartida, int numeroImagenes = 1)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            try
            {
                var contexto = contextoOperacion.GetCallbackChannel<IImagenMazoCallback>();
                partidasDiccionario.TryGetValue(idPartida, out Partida partida);
                if (partida != null)
                {
                    IMediadorImagen mediador = partida.mediadorImagen;
                    ManejadorImagen manejadorImagen = new ManejadorImagen(
                        Escritor,
                        mediador,
                        partida.tematica,
                        new LectorDisco(),
                        partida.ImagenesTablero.Values.SelectMany(lista => lista).ToList());
                    List<(string RutaCompleta, string NombreArchivo)> resultado = mediador.ObtenerMultiplesRutasYNombres(numeroImagenes);
                    List<string> rutasCompletas = resultado.Select(tupla => tupla.RutaCompleta).ToList();
                    LecturaTrabajo lecturaTrabajo = new LecturaTrabajo(rutasCompletas, contexto);
                    return await manejadorImagen.SolicitarImagenMazoAsync(lecturaTrabajo);
                }
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

