using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class JugadorEstadisticas
    {
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int Puntos { get; set; }
        public JugadorEstadisticas(string nombre)
        {
            Nombre = nombre;
            Puntos = 0;
        }
        public JugadorEstadisticas(string nombre, int  puntos)
        {
            Nombre = nombre;
            Puntos = puntos;
        }
    }
}