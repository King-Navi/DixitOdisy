using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCliente.Utilidad
{
    public class SingletonAmigos
    {
        // Campo estático para almacenar la única instancia de la clase
        private static readonly Lazy<SingletonAmigos> instancia = new Lazy<SingletonAmigos>(() => new SingletonAmigos());
        //private static ConcurrentBag<>
        public static SingletonAmigos Instancia => instancia.Value;

        // Constructor privado para evitar la creación de instancias desde fuera
        private SingletonAmigos() { }

        // Ejemplo de un método o propiedad dentro del Singleton
        public void AgregarAmigo(string nombreAmigo)
        {
            Console.WriteLine($"Amigo {nombreAmigo} agregado.");
        }

        public void ListarAmigos()
        {
            Console.WriteLine("Listando amigos...");
            // Implementa la lógica para listar amigos
        }
    }
}
