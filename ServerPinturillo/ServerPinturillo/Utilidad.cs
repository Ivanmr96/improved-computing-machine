using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo
{
    public class Utilidad
    {



        //private static String[] listadoPalabras = { "Ratón y mouse", "Teclado y otra cosa" };

        private static String[] listadoPalabras = {"Ratón", "Teclado", "Monitor", "Profesor", "Movil", "Océano", "Mar", "Playa", "Persona", "Igual",
        "Saludar", "Hucha","Libro", "Vaso", "Plátano", "Ventana", "Pañuelo", "Gato", "Perro", "Loro", "Dignidad",
        "Mochila", "Muralla", "Android", "Cámara", "Brazo", "Hilo", "Twitter", "Instagram","Barco","Estrella",
        "Youtube", "Música", "Escaleras", "Platos", "Cocina", "Ojo", "Cine", "Butaca","Cohete","Selva",
        "Viaje","Jazz","Caballo","Tortuga","Elefante","Mapache","Mono","Pulpo","Cine","Mar","Globo",
        "Rectángulo","Círculo","Ordenador","Pizarra","Darth Vader","Iron man","Capitán América","Hulk",
        "Superman","Batman","Thor","Spiderman","Thanos","Flash","Cebolla"};

        public static String obtenerPalabraAleatoria()
        {
            Random rnd = new Random();
            int i = rnd.Next(listadoPalabras.Length);

            return listadoPalabras[i];

        }



        //Método que rellena el listado de posiciones a descubrir
        public static List<int> rellenarPosicionesADescubrir(string palabraAJugar)
        {
            int maximosCaracteresADescubrir = palabraAJugar.Length - 2;

            List<int> listNumbers = new List<int>();
            int number;
            Random rnd = new Random();

            if(maximosCaracteresADescubrir > 0)
            {
                for (int i = 0; i < maximosCaracteresADescubrir; i++)
                {
                    do
                    {
                        number = rnd.Next(maximosCaracteresADescubrir);
                    } while (listNumbers.Contains(number));
                    listNumbers.Add(number);
                }

            }
            else
            {
                listNumbers.Add(0);
            }

            return listNumbers;
        }
    }
}