using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// Esta clase hace referncia al modelo de usurio en nuestro caso va a ser el de base de datos
    /// </summary>
    [DataContract]
    public class Usuario : UsuarioContexto
    {
        #region Campos
        private SHA256 contraseniaHASH;
        private String correo;
        private Enumerador.EstadoJugador estadoJugador;
        private Stream fotoUsuario;
        private int idUsuario;

        #endregion Campos

        #region Propiedades
        [DataMember]
        public int IdUsuario1 { get => idUsuario; set => idUsuario = value; }
        [DataMember]
        public SHA256 ContraseniaHASH { get => contraseniaHASH; set => contraseniaHASH = value; }
        [DataMember]
        public string Correo { get => correo; set => correo = value; }
        [DataMember]
        public Stream FotoUsuario { get => fotoUsuario; set => fotoUsuario = value; }

        #endregion Propiedades

        #region Metodos
        public override string ToString()
        {
            return Nombre;
        }
        #endregion Metodos
    }
}
