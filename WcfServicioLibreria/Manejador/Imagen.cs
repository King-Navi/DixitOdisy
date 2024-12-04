using System;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;
using WcfServicioLibreria.Modelo;
using WcfServicioLibreria.Utilidades;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioImagen
    {
        public async Task<bool> SolicitarImagenCartaAsync(string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            try
            {
                var contexto = contextoOperacion.GetCallbackChannel<IImagenCallback>();
                manejadoresImagenes.TryGetValue(idPartida, out ManejadorImagen manejadorImagen);
                return await manejadorImagen.EnviarImagen(contexto);
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }
        public bool MostrarTodasImagenes(string idPartida)
        {
            if (!ValidarPartida(idPartida))
            {
                return false;
            }
            try
            {
                var contexto = contextoOperacion.GetCallbackChannel<IImagenCallback>();
                manejadoresImagenes.TryGetValue(idPartida, out ManejadorImagen manejadorImagen);
                manejadorImagen.MostrarGrupoCartas(contexto);
                return true;
            }
            catch (ArgumentNullException excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarErrorException(excepcion);
            }
            return false;
        }
    }
}
