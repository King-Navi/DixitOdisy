//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAOLibreria.ModeloBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class PeticionAmistad1
    {
        public int idPeticion { get; set; }
        public int idRemitente { get; set; }
        public int idDestinatario { get; set; }
        public Nullable<System.DateTime> fechaPeticion { get; set; }
        public string estado { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
    }
}