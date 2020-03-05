using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinturillo.Utils
{
    public class Util
    {
        /// <summary>
        /// Formatea una palabra que está invisible para el usuario, mostrándo la letra de la posición dada
        /// </summary>
        /// <param name="palabra">La palabra que está invisible al usuario</param>
        /// <param name="palabraOriginal">La palabra original</param>
        /// <param name="nuevaPosicionADescubrir">La posición de la letra a descubrir</param>
        /// <returns></returns>
        public static string obtenerPalabraFormateada(string palabra,string palabraOriginal, int nuevaPosicionADescubrir)
        {
            StringBuilder nuevaPalabra = new StringBuilder(palabra);    //se tiene que usar esto del string builder
            for(int i =0; i < palabra.Length; i++)
            {
                if (!palabra[i].Equals("*") && i == nuevaPosicionADescubrir)
                {
                    nuevaPalabra[i] = palabraOriginal[i];       //para poder hacer esto
                }
            }

            return nuevaPalabra.ToString();
        }
    }
}
