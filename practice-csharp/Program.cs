using System;
using System.Collections.Generic;

namespace Ejercicio1_PalabraMasRepetida_SinLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            string phrase = "     Tony has learn a lot, about C# and Tony loves it!";

            PalabraMasRepetida(phrase);
        }

        static void PalabraMasRepetida(string phrase)
        {
            // ---------------------------------------------------------
            // PASO 1: Limpiar signos de puntuación, letra por letra.
            // En vez de Where(...), recorremos el string con un foreach
            // y vamos construyendo un nuevo string solo con lo que sirve.
            // ---------------------------------------------------------
            string limpio = "";
            foreach(char c in phrase)
            {
                if(char.IsLetter(c) || c != ' ') limpio += c;
            }

            // ---------------------------------------------------------
            // PASO 2: Separar en palabras.
            // Split sin RemoveEmptyEntries puede dejar strings vacíos
            // (por ejemplo si quedaron dos espacios seguidos donde
            // antes había una coma). Los filtramos nosotros mismos.
            // ---------------------------------------------------------
            string[] partes = limpio.Split(' ');
            Console.WriteLine($"texto en partes: {partes}");

            // ---------------------------------------------------------
            // PASO 3: Contar apariciones con un diccionario, sin
            // GetValueOrDefault. Usamos ContainsKey para decidir
            // si ya existe la clave o hay que crearla.
            // ---------------------------------------------------------
            Dictionary<string, int> conteo = new Dictionary<string, int>();

            foreach (string parte in partes)
            {
                if (parte == "")
                {
                    continue; // saltamos strings vacíos (el reemplazo de RemoveEmptyEntries)
                }

                string palabra = parte.ToLower(); // para que "Tony" y "tony" cuenten igual

                if (conteo.ContainsKey(palabra))
                {
                    conteo[palabra] +=1;
                }
                else
                {
                    conteo[palabra] = 1;
                }
            }

            // ---------------------------------------------------------
            // PASO 4: Recorrer el diccionario y encontrar el valor máximo.
            // ---------------------------------------------------------
            string masRepetida = "";
            int max = 0;

            foreach (KeyValuePair<string, int> par in conteo)
            {
                if (par.Value > max)
                {
                    max = par.Value;
                    masRepetida = par.Key;
                }
            }

            // ---------------------------------------------------------
            // Resultados
            // ---------------------------------------------------------
            Console.WriteLine("Frase original: " + phrase);
            Console.WriteLine("Frase limpia:   " + limpio);
            Console.WriteLine();
            Console.WriteLine("Conteo de cada palabra:");
            foreach (KeyValuePair<string, int> par in conteo)
            {
                Console.WriteLine("  " + par.Key + ": " + par.Value);
            }
            Console.WriteLine();
            Console.WriteLine("Palabra mas repetida: '" + masRepetida + "' (" + max + " veces)");
        }
    }
}