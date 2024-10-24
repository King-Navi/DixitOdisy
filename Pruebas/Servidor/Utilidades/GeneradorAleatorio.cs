﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Modelo;

namespace Pruebas.Servidor.Utilidades
{
    public static class GeneradorAleatorio
    {
        public static MemoryStream GenerarStreamAleatorio(int longitud)
        {
            byte[] entrada = new byte[longitud];
            Random aleatorio = new Random();
            aleatorio.NextBytes(entrada); // Llena el buffer con datos aleatorios

            return new MemoryStream(entrada);
        }
        public static Usuario GenerarUsuarioAleatorio()
        {
            string nombre = "JugadorPrueba" + new Random().Next(1000, 9999);
            return new Usuario {
                Nombre = nombre,
                ContraseniaHASH = DAO.Utilidad.ObtenerSHA256Hash("Contraseña" + new Random().Next(1000, 9999)),
                Correo = $"{nombre.ToLower()}@gmail.com",
                FotoUsuario = GeneradorAleatorio.GenerarStreamAleatorio(20)
            };
        }
    }
}