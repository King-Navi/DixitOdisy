//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAOLibreria.ModeloBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Amigo = new HashSet<Amigo>();
            this.Amigo1 = new HashSet<Amigo>();
            this.Estadisticas = new HashSet<Estadisticas>();
            this.PeticionAmistad = new HashSet<PeticionAmistad>();
            this.PeticionAmistad1 = new HashSet<PeticionAmistad>();
        }
    
        public int idUsuario { get; set; }
        public string gamertag { get; set; }
        public byte[] fotoPerfil { get; set; }
        public Nullable<System.DateTime> ultimaConexion { get; set; }
        public int idUsuarioCuenta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Amigo> Amigo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Amigo> Amigo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estadisticas> Estadisticas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PeticionAmistad> PeticionAmistad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PeticionAmistad> PeticionAmistad1 { get; set; }
        public virtual UsuarioCuenta UsuarioCuenta { get; set; }
    }
}
