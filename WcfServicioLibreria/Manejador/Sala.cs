using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfServicioLibreria.Contratos;

namespace WcfServicioLibreria.Manejador
{
    public partial class ManejadorPrincipal : IServicioSala
    {
        public bool BorrarSala(string idSala)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Metodo que genera una sala
        /// </summary>
        /// <returns></returns>
        public string CrearSala(string nombreUsuarioAnfitrion)
        {
            string resultado = null;
            try
            {
                string idSala;
                do
                {
                    idSala = GenerarIdUnico();
                } while (salasDiccionario.ContainsKey(idSala));

                bool existeSala = salasDiccionario.TryAdd(idSala, new Modelo.Sala(idSala, nombreUsuarioAnfitrion));
                if (!existeSala)
                {
                    resultado = null;
                }
                else
                {
                    resultado = idSala;
                }
            }
            catch (CommunicationException excepcion)
            {
                //TODO: Manejar el error
            }
            return resultado;
        }
        /// <summary>
        /// metodo que genera un id alfanumerico de 6 caracteres que no este en uso para una sala
        /// </summary>
        /// <returns></returns>
        public string GenerarIdUnico()
        {
            const string CARACTERES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int LONGITUD_ID = 6;
            StringBuilder resultado = new StringBuilder(LONGITUD_ID);
            try
            {
                byte[] datosAleatorios = new byte[LONGITUD_ID];

                using (var generador = RandomNumberGenerator.Create())
                {
                    generador.GetBytes(datosAleatorios);
                }

                for (int i = 0; i < LONGITUD_ID; i++)
                {
                    int indice = datosAleatorios[i] % CARACTERES.Length;
                    resultado.Append(CARACTERES[indice]);
                }
            }
            catch (CommunicationException ex)
            {
                //TODO: Manejar el error
            }
            return resultado.ToString();
        }
        /// <summary>
        /// Valida si existe una sala con ese idSala
        /// </summary>
        /// <param name="idSala"></param>
        /// <returns></returns>
        public bool ValidarSala(string idSala)
        {
            bool result = false;
            try
            {
                result = salasDiccionario.ContainsKey(idSala);
            }
            catch (CommunicationException ex)
            {
                   //TODO manejar el error
            }
            return result;
        }
    }
}
