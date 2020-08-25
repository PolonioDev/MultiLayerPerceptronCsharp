// using System.Collections;
// using System.IO;
using System;
// using System.Text.Json.Serialization;
// using System.Collections.Generic;
using NeuronalNetwok.Dataset_Manager;

namespace MultiLayerPerceptron
{
    class Program
    {
        static void Main(string[] args)
        {
            Dataset xnor = new Dataset("./xnor_dataset.csv",2,1); // Read the dataset 
            // Con esta opcion leemos un archivo .csv con los datos de entrenamiento 


            MultiLayerPerceptron p = new MultiLayerPerceptron(new int[3]{2,2,1}); // Create Multi-Layer Perceptron
            // Con esta opcion se crea un perceptron nuevo 


            // MultiLayerPerceptron p = new MultiLayerPerceptron(new int[3]{2,2,1}, "./perceptron.json"); // Create or Read serialized Multi-Layer Perceptron
            // Con esta opcion creamos un perceptron con archivo de volcado, en caso de existir el archivo lo lee y crea el perceptron
            //que ha sido guardado en ese archivo, si no existe el archivo especificado, lo guarda para poder guardar el modelo con la funcion save() de la clase MultiLayerPerceptron

            // p.show_model(); // show the generated model
          
            p.Learn(xnor, 0.3); // learn xnor rule
            // Con esta funcion entrenas al perceptron para aprender algo
            // El segundo paramentro se llama tasa o factor de aprendizaje, entre mas bajo mas exacto es el modelo pero mas dificil de entrenar se vuelve el trabajo.

            // show all posibilities
            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    Console.WriteLine("E{0},E{1} = {2} = {3}", i, j, Math.Round(p.stimulate(new double[2]{i,j})[0]), p.stimulate(new double[2]{i,j})[0]);
                }  
            } 
            // Con este codigo le preguntas al perceptron el condicional XNOR

            // p.save(); // save current perceptron
            //Si has creado un perceptron con un archivo de volcado (Como el segundo ejemplo para crear el percetron) utilizas esta funcion para guardar el modelo actual y poder usarlo sin entrenar la siguinte vez que ejecutes el programa

            // p.save("./mi_perceptron.json");
            // Con esta opcion guardas el modelo actual en el archivo especificado 

            // Console.ReadKey(); // wait for close program
        }
    }
}
