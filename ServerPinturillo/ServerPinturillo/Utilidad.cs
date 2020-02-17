using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo
{
    public class Utilidad
    {
        private static String[] listadoPalabras = {"Ratón", "Teclado", "Monitor", "Profesor", "Movil", "Océano", "Mar", "Playa", "Persona", "CRUD", "FernandoApruebanos", 
                                                    "Xamarin", "Hucha","Libro", "Vaso", "Plátano", "Ventana", "Pañuelo", "Gato", "Perro", "Loro", "Dignidad", 
                                                     "Mochila", "Iván Cállate", "Android", "Cámara", "Brazo", "Hilo", "Twitter", "Instagram", 
                                                    "Youtube", "Música", "Escaleras", "Platos", "Cocina", "Ojo", "Cine", "Butaca"};

        public static String obtenerPalabraAleatoria()
        {
            Random rnd = new Random();
            int i = rnd.Next(listadoPalabras.Length);

            return listadoPalabras[i];

        }
    }
}