using System;
using System.IO;
using System.Runtime.Serialization;
using WcfServicioLibreria.Enumerador;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    public class Amigo
    {
        [DataMember]
        public Stream Foto { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public Enumerador.EstadoAmigo Estado { get; set; }


        public Amigo() { }

        public Amigo(Stream foto, string nombre, EstadoAmigo estado)
        {
            Foto = foto;
            Nombre = nombre;
            Estado = estado;
        }

        public Amigo(string nombre, EstadoAmigo estado)
        {
            Nombre = nombre;
            Estado = estado;
        }
    }
}
