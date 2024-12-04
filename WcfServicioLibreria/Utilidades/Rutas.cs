using System;
using System.IO;
using WcfServicioLibreria.Enumerador;

namespace WcfServicioLibreria.Utilidades
{
    public static class Rutas
    {
        public const string RUTA_RECURSOS = "Recursos";
        public const string CARPETA_MIXTA = "Mixta";
        public const string CARPETA_MITOLOGIA = "Mitologia";
        public const string CARPETA_ANIMALES = "Animales";
        public const string CARPETA_PAISES = "Paises";
        public const string CARPETA_ESPACIO = "Espacio";
        public const string EXTENSION_TODO_ARCHIVO_JPG = "*.jpg";
        public const string EXTENSION_TODO_ARCHIVO_PNG = "*.png";
        public const string CARPETA_FOTOS_INVITADOS = "FotosInvitados";

        public static string CalcularRutaImagenes(TematicaPartida tematica)
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RUTA_RECURSOS);
            switch (tematica)
            {
                case TematicaPartida.Mixta:
                    return Path.Combine(ruta, CARPETA_MIXTA);
                case TematicaPartida.Mitologia:
                    return Path.Combine(ruta, CARPETA_MITOLOGIA);
                case TematicaPartida.Animales:
                    return Path.Combine(ruta, CARPETA_ANIMALES);
                case TematicaPartida.Paises:
                    return Path.Combine(ruta, CARPETA_PAISES);
                case TematicaPartida.Espacio:
                    return Path.Combine(ruta, CARPETA_ESPACIO);
                default:
                    return Path.Combine(ruta, CARPETA_MIXTA);
            }
        }
    }
}
