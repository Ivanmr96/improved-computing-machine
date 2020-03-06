using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerPinturillo
{
    public class Utilidad
    {



        //private static String[] listadoPalabras = { "Ratón y mouse", "Teclado y otra cosa" };

        private static String[] listadoPalabras = {

        "Ratón", "Teclado", "Monitor", "Profesor", "Movil", "Océano", "Mar", "Playa", "Persona", "Igual", "Saludar",
            "Hucha","Libro", "Vaso", "Plátano", "Ventana", "Pañuelo", "Gato", "Perro", "Loro", "Dignidad",
        "Mochila", "Muralla", "Android", "Cámara", "Brazo", "Hilo", "Twitter", "Instagram","Barco","Estrella",
            "Youtube", "Música", "Escaleras", "Platos", "Cocina", "Ojo", "Cine", "Butaca","Cohete","Selva",
        "Viaje","Jazz","Caballo","Tortuga","Elefante","Mapache","Mono","Pulpo","Cine","Mar","Globo", "Rectángulo",
            "Círculo","Ordenador","Pizarra","Darth Vader","Ironman","Capitán América","Hulk", "Superman","Batman",
            "Thor","Spiderman","Thanos","Flash","Cebolla", "Tarjeta", "Pegatina", "Cristal", "Mopa", "Lápiz", "Saltamontes",
            "Hormiga", "Proyector", "Pimiento", "Autobus", "Coche", "Vaca", "Burro",
        "Cacatúa", "Luna", "Amor", "Corazón", "Melón", "Sandía", "Kiwi", "Foto", "Camiseta", "Pantalones", "Atenea", "Zeus",
            "Lavadora", "Lavavajillas", "Fregadero", "Sofá", "Padre", "Dios", "Madre", "Botella",
        "Tomate", "Columna", "Fila", "Nevera", "Taza", "Cuenco", "Arroz", "Lechuga", "Aire acondicionado", "Calentador", "Cuadro",
            "Televisión", "Portugal", "España", "Francia", "Silla", "Sevilla",
        "Agujero negro", "Palmera", "Moflete", "Imán", "Vacuna", "Planeta", "Tierra", "Espacio", "Sol", "Miel", "Clavo", "Espectador",
            "Campo", "Medalla", "Frambuesa", "Hamburguesa", "Deportista", "Antebrazo", "Cabeza","Cuerpo", "Tórax",
        "Pulmón", "Plumas", "Grillo", "Cojín", "Pájaro", "Estornino", "León", "Guepardo", "Gacela", "Leopardo", "Buitre",
            "Ave carroñera", "Cielo", "Nube", "Bisagra", "Destornillador", "Anillo", "Boda", "Comunión", "Viaje", "Pulso",
            "Pulsera", "Agenda", "Cable", "Hoja",
        "Árbol", "Madera", "Flor", "Rosa", "Naranja", "Comedia", "Dragón", "Sangre", "Piel", "Ser humano", "Apisonadora",
            "Película", "Pelícano", "Gorrión", "Frío", "Calor", "Templado", "Templo", "Iglesia", "Mezquita", "Cerrado", "Enero",
            "Mes", "Tiempo", "Legendario", "Leyenda",
        "Mastodonte", "Fiera", "Sable", "Atajo", "Tubérculo", "Patata", "Periscopio", "Cangrejo", "Concha", "Dinamita",
            "Navidad", "Frente",
        "Carrera", "Trucha", "Lobo", "Asesinato", "Asesino", "Muerte", "Enfermedad", "Anciano", "Consola", "Audífono", "Reno", "Papá Noel",
        "Ombligo", "Disquete", "Cronómetro", "Juego", "Termostato", "Sello", "Fiesta", "Lejos", "Cerca", "Barrera", "Hospital", "Virus", "Juego de mesa", "Mesa",
        "Rastro", "Rata", "Careta", "Carnaval", "Batidora", "Horno", "Microondas", "Instrumentos", "Saxofón", "Piano", "Química", "Física", "Matemáticas",
        "Lengua", "Biología", "Informática", "Juicio", "Primero", "Gasa", "Norte", "Sur", "Este", "Oeste", "Rosa de los vientos", "Vista", "Oreja",
        "Helado", "Remolque", "Motor", "Gotera", "Gota", "Paella", "Lava", "Monja", "Cura", "Mermelada", "Llanto", "Llorar", "Sacrificio",
            "Barco pesquero", "Veneno", "Tóxico", "Calavera", "Hueso", "Cocodrilo", "Calculadora", "Cómic", "Carta", "Mensaje", "Espejo", "Espejismo"
                

        };


        /// <summary>
        /// Método que devuelve una palabra
        /// aleatoria del array de palabras
        /// </summary>
        /// <returns>palabra del array de palabras</returns>
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