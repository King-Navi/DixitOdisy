﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLibreria.Excepciones
{
    /// <summary>
    /// 
    /// </summary>
    /// <ref>https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions#define-exception-classes</ref>
    public class PropiedadesNulasExcepcion : Exception
    {
        public PropiedadesNulasExcepcion() { }
        public PropiedadesNulasExcepcion(string message) : base(message) { }
        public PropiedadesNulasExcepcion(string message, Exception innerException) : base(message, innerException) { }

        public static bool TodasPropiedadesNoNulas<T>(T instancia) where T : class
        {
            if (instancia == null)
                throw new PropiedadesNulasExcepcion(nameof(instancia));
            var propiedades = typeof(T).GetProperties();
            foreach (var propiedad in propiedades)
            {
                var valor = propiedad.GetValue(instancia);
                if (propiedad.GetIndexParameters().Any())
                    continue;

                if (valor == null)
                    return false;
            }
            return true;
        }
    }
}