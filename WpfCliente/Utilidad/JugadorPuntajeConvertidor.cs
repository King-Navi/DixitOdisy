using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCliente.Properties;

namespace WpfCliente.Utilidad
{
    public class JugadorPuntajeConvertidor
    {
        public static List<JugadorTablaPuntaje> ConvertirAListaJugadorTablaPuntaje(List<ServidorDescribelo.JugadorPuntaje> listaOriginal, bool sePuedeVer)
        {
            try
            {
                if (String.IsNullOrEmpty(SingletonCliente.Instance.NombreUsuario))
                {
                    throw new ArgumentException();
                }
                if (listaOriginal == null)
                {
                    throw new ArgumentException();
                }
                return listaOriginal.Select(busqueda => new JugadorTablaPuntaje
                {
                    Nombre = busqueda.Nombre.Equals(SingletonCliente.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase) ? $"{busqueda.Nombre} {Idioma.labelTu}" : busqueda.Nombre,
                    Puntos = busqueda.Puntos,
                    MostrarBoton = busqueda.Nombre.Equals(SingletonCliente.Instance.NombreUsuario, StringComparison.OrdinalIgnoreCase) ? false : sePuedeVer
                }).ToList();
            }
            catch (Exception excepcion)
            {
                ManejadorExcepciones.ManejarExcepcionErrorComponente(excepcion);
            }
            return new List<JugadorTablaPuntaje>();

        }
    }
}
