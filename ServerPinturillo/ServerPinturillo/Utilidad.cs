using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo
{
    public class Utilidad
    {
        private String[] listadoPalabras = {"Raton", "Teclado", "Monitor", "Profesor", "Movil" };

        public static String obtenerPalabraAleatoria()
        {
            Random rnd = new Random();
            int i = rnd.Next(listadoPalabras.Length);

            return listadoPalabras[i];

        }
    }
}