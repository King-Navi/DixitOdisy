using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UtilidadesLibreria;

namespace PruebasUnitariasCliente
{
    [TestClass]
    public class ValidarContraseniaPruebas
    {
        [TestMethod]
        public void ValidarContrasenia_ContraseniaVacia_DebeRetornarFalso()
        {
            // Arrange
            string contrasenia = string.Empty;

            // Act
            Dictionary<string, object> resultado = WpfCliente.Validacion.ValidarContrasenia(contrasenia);

            // Assert
            Assert.IsFalse((bool)resultado[Llaves.LLAVE_BOOLEANO]);
            Assert.AreEqual("Es vacio", resultado[Llaves.LLAVE_MENSAJE]);
        }

        [TestMethod]
        public void ValidarContrasenia_LongitudInvalida_DebeRetornarFalso()
        {
            // Arrange
            string contrasenia = "abc"; // Menos de 5 caracteres

            // Act
            Dictionary<string, object> resultado = WpfCliente.Validacion.ValidarContrasenia(contrasenia);

            // Assert
            Assert.IsFalse((bool)resultado[Llaves.LLAVE_BOOLEANO]);
            Assert.AreEqual("La contraseña debe tener entre 5 y 20 caracteres.", resultado[Llaves.LLAVE_MENSAJE]);
        }

        [TestMethod]
        public void ValidarContrasenia_CaracteresInvalidos_DebeRetornarFalso()
        {
            // Arrange
            string contrasenia = "abcd@"; // '@' no es un carácter permitido

            // Act
            Dictionary<string, object> resultado = WpfCliente.Validacion.ValidarContrasenia(contrasenia);

            // Assert
            Assert.IsFalse((bool)resultado[Llaves.LLAVE_BOOLEANO]);
            Assert.AreEqual("La contraseña contiene caracteres no permitidos.", resultado[Llaves.LLAVE_MENSAJE]);
        }

        [TestMethod]
        public void ValidarContrasenia_Valida_DebeRetornarVerdadero()
        {
            // Arrange
            string contrasenia = "abcd1234#$%&"; // Contraseña válida

            // Act
            Dictionary<string, object> resultado = WpfCliente.Validacion.ValidarContrasenia(contrasenia);

            // Assert
            Assert.IsTrue((bool)resultado[Llaves.LLAVE_BOOLEANO]);
            Assert.AreEqual("La contraseña es válida.", resultado[Llaves.LLAVE_MENSAJE]);
        }
    }
}
