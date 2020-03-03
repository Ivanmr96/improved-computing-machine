using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinturilloParaPruebas3.Utils
{
    public class Util
    {
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
