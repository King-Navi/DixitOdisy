﻿using System.IO;
using System.Runtime.Serialization;
using WcfServicioLibreria.Enumerador;
namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(Stream))]
    [KnownType(typeof(FileStream))]
    [KnownType(typeof(MemoryStream))]
    [KnownType(typeof(EstadoAmigo))]
    public class Amigo
    {
        [DataMember]
        public byte[] Foto { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string UltimaConexion { get; set; }
        [DataMember]
        public Enumerador.EstadoAmigo Estado { get; set; }


        public Amigo() { }

        public Amigo(byte[] foto, string nombre, EstadoAmigo estado)
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
