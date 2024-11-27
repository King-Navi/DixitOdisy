namespace DAOLibreria.ModeloBD
{
    public class UsuarioPerfilDTO
    {
        
            public int IdUsuario { get; set; }
            public int IdUsuarioCuenta { get; set; }
            public string NombreUsuario { get; set; }
            public string Correo { get; set; }
            public byte[] FotoPerfil { get; set; }
            public string HashContrasenia { get; set; }

        public UsuarioPerfilDTO() { }
        public UsuarioPerfilDTO(Usuario usuario, UsuarioCuenta cuenta)
        {
            IdUsuario = usuario.idUsuario;
            NombreUsuario = usuario.gamertag;
            Correo = cuenta.correo;
            FotoPerfil = usuario.fotoPerfil;
            HashContrasenia = cuenta.hashContrasenia;
            IdUsuarioCuenta = cuenta?.idUsuarioCuenta ?? 0;
        }

    }
}
