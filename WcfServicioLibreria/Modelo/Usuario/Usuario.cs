using System;
using System.IO;
using System.Runtime.Serialization;
using WcfServicioLibreria.Enumerador;

namespace WcfServicioLibreria.Modelo
{
    [DataContract]
    [KnownType(typeof(Stream))]
    [KnownType(typeof(FileStream))]
    [KnownType(typeof(MemoryStream))]
    [KnownType(typeof(EstadoUsuario))]

    public class Usuario : UsuarioContexto
    {
        #region Campos
        private string contraseniaHASH;
        private String correo;
        private Enumerador.EstadoUsuario estadoJugador;
        private Stream fotoUsuario;

        public Usuario() { }

        public Usuario(string _nombre, Stream _fotoUsuario)
        {
            fotoUsuario = _fotoUsuario;
            nombre = _nombre;
        }

        #endregion Campos

        #region Propiedades
        [DataMember]
        public string Nombre { get => nombre; set => nombre = value; }
        [DataMember]
        public int IdUsuario { get => idUsuario; set => idUsuario = value; }
        [DataMember]
        public string ContraseniaHASH { get => contraseniaHASH; set => contraseniaHASH = value; }
        [DataMember]
        public string Correo { get => correo; set => correo = value; }
        [DataMember]
        public Stream FotoUsuario { get => fotoUsuario; set => fotoUsuario = value; }
        [DataMember]
        public bool EsInvitado { get; set; }
        public EstadoUsuario EstadoJugador { get => estadoJugador; set => estadoJugador = value; }

        #endregion Propiedades

        #region Metodos
        public override string ToString()
        {
            return Nombre;
        }
        #endregion Metodos
    }
}
