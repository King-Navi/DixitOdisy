using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WcfServicioLibreria.Modelo
{
    /// <summary>
    /// Esta clase hace referncia al modelo de usurio en nuestro caso va a ser el de base de datos
    /// </summary>
    [DataContract]
    public class Usuario : UsuarioContexto
    {
        #region Campos
        private int idUsuarioCuenta;
        private String nombre;
        private SHA256 contraseniaHASH;
        private String correo;
        private Enumerador.EstadoJugador estadoJugador;

        #endregion Campos

        #region Propiedades
        [DataMember]
        public string Nombre { get => nombre; set => nombre = value; }
        [DataMember]
        public int IdUsuario { get => idUsuarioCuenta; set => idUsuarioCuenta = value; }
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
