using System;
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

        #endregion Campos

        #region Propiedades
        [DataMember]
        public SHA256 ContraseniaHASH { get => contraseniaHASH; set => contraseniaHASH = value; }
        #endregion Propiedades

        #region Metodos
        public override string ToString()
        {
            return Nombre;
        }
        #endregion Metodos
    }
}
