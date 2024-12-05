using System.Runtime.Serialization;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class JugadorPuntaje
    {
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int Puntos { get; set; }
        public JugadorPuntaje(string nombre)
        {
            Nombre = nombre;
            Puntos = 0;
        }
        public JugadorPuntaje(string nombre, int  puntos)
        {
            Nombre = nombre;
            Puntos = puntos;
        }
    }
}