using DAOLibreria;
using DAOLibreria.DAO;
using DAOLibreria.ModeloBD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Pruebas.DAO.Utilidades
{
    public abstract class ConfiguracionPruebaBD
    {
        public const string NOMBRE_USUARIO_EXISTENTE = "UsuarioPredeterminado";
        public const int ID_INVALIDO = -1;
        public const int ID_INEXISTENTE = 9999;
        public const int ID_VALIDO = 1;
        public const string NOMBRE_USUARIO_INEXISTENTE = "UsuarioInexistenteEnBasaDatos";
        public const string NUEVA_CONTRASENIA = "NuevaContraseña";
        public Usuario usuarioInicial;
        public UsuarioCuenta usuarioCuentaInicial;
        public const string POR_DEFECTO_NOMBRE = "UsuarioPredeterminado";
        public const string POR_DEFECTO_NOMBRE_CUENTA = POR_DEFECTO_NOMBRE;
        public const string POR_DEFECTO_CORREO = "default@example.com";
        public const string POR_DEFECTO_CONTRANIA_HASH = "6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B";
        public static readonly byte[] POR_DEFECTO_FOTO_PERFIL = new byte[] { };
        public static readonly DateTime POR_DEFECTO_ULTIMA_CONEXION = DateTime.Now;
        public const int POR_DEFECTO_IDUSUARIO_CUENTA = 1;
        public const string CONTRASENIA_INCORRECTA = "1234";
        public UsuarioDAO usuarioDAO = new UsuarioDAO();
        public const int POR_DEFECTO_IDUSUARIO_CUENTA_SEGUNDO = 2;
        public const int POR_DEFECTO_IDUSUARIO_CUENTA_TERCERO = 3;
        public const string POR_DEFECTO_NOMBRE_SEGUNDO = "Jugador2";
        public const string POR_DEFECTO_NOMBRE_TERCERO = "Jugador3";

        [TestInitialize]
        public virtual void ConfigurarPruebas()
        {
            var resultado = ConfiguradorConexion.ConfigurarCadenaConexion("localhost", "Describelo", "devDescribelo", "UnaayIvan2025@-");
            if (!resultado)
            {
                Assert.Fail("La BD no está configurada.");
            }
        }
    }
}
