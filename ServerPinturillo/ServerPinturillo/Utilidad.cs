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
                                                    "Youtube", "Música", "Escaleras", "Platos", "Cocina", "Ojo", "Cine",

                                                    "Gato",
"Perro",
"Remo",
"Escenario",
"Helado",
"Canadá",
"Alarma",
"Reloj",
"Ladrón",
"Laringe",
"Bomba",
"Trapo",
"Guirnalda",
"Moqueta",
"Bote",
"Gallina",
"Crucero",
"Billar",
"Remolque",
"Coche",
"Terminal",
"Cafetera",
"Pollo",
"Saltamontes",
"Insecto",
"Sudamérica",
"Vagabundo",
"Kiwi",
"Rocas",
"Galleta",
"Cuadros",
"Envase",
"Copa",
"Luna de miel",
"Minigolf",
"Ardilla",
"Privado",
"Adivino",
"Ganado",
"Tulipán",
"Amor",
"Terremoto",
"Eructo",
"Queso",
"Chimenea",
"Papada",
"Cuaderno",
"Sacerdote",
"Tablero",
"Congreso",
"Carga",
"Doctor",
"Serrucho",
"Granjero",
"Java",
"CSharp",
"Marinero",
"Asado",
"Tribu",
"Tripi",
"Misionero",
"Paciente",
"Medicina",
"Cura",
"Esteroides",

            "Butaca" };

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