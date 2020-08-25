using System.Linq;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using NeuronalNetwok;
using NeuronalNetwok.Dataset_Manager;

namespace MultiLayerPerceptron
{
   public class MultiLayerPerceptron : NuronalNetwork_Manager
   {
      public string Model_File_Path;
      public bool SaveOn_GarbageCollector;
      public static int[] AUTO_DETECT_MODEL = new int[1]{202020};

       public MultiLayerPerceptron(int[] structure, string Model_File_Path="", bool Load_Model_File = true, bool SaveOn_GarbageCollector=true) : base(structure){
          try
          {
             Model_File_Path = Path.GetFullPath(Model_File_Path);
            if(Path.IsPathRooted(Model_File_Path)){
               this.SaveOn_GarbageCollector = SaveOn_GarbageCollector;
               this.Model_File_Path = Model_File_Path;
               bool Model_auto = (string.Join(',',structure) == string.Join(",",AUTO_DETECT_MODEL))?true:false;
               if(Load_Model_File)
                  this.load(Model_File_Path,Model_auto);
            }else{
               throw new Exception("Use default parameters");
            }
          }
          catch (System.Exception)
          {
            this.Model_File_Path = "";
            this.SaveOn_GarbageCollector = false;
          }
       }

      public bool Learn(Dataset dataset, double learning_rate=0.2, double max_error_allowed=0.01)
      {
         return this.Learn(dataset.inputs, dataset.answers, learning_rate, max_error_allowed);
      }
      public bool Learn(List<double[]> inputs, List<double[]> answers, double learning_rate=0.2, double max_error_allowed=0.01){
         double error = 0;
         int i = 0;
         Console.WriteLine("Error Inicial: {0}", getGlobalError(inputs, answers));
         while(i==0 || (error > max_error_allowed)){

            i++;
            learn_this(inputs, answers, learning_rate);
            error = getGlobalError(inputs, answers);
            if(i%1500==0){
               Console.WriteLine("Error: {0}", error); 
            }
               

            if(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape){
               return false;
            }           
         }
         return true;
      }
      public void learn_this(List<double[]> inputs, List<double[]> answers, double learning_rate=0.2)
      {
         // Initialize Learning Data
         walk_in(delegate(int layer, int neuron, int input){
            if(input < 0){
               sigma(layer, neuron, 0);
            }else{
               delta(layer, neuron, input, 0);
            }
         }, true);
         for (int i = 0; i < inputs.Count; i++)
         {
             this.stimulate(inputs[i]);
            //  Set Sigmas
             walk_in(delegate(int layer, int neuron, int any){
               if(layer == layers_count-1){
                  sigma(layer, neuron, ((last_output(layer,neuron)-answers[i][neuron]) * derived(layer, neuron)));
               }else{
                  double summation = 0;
                  for (int next_layer_neuron = 0; next_layer_neuron < neurons_count(layer+1); next_layer_neuron++)
                  {
                      summation += sigma(layer+1, next_layer_neuron) * weight(layer+1, next_layer_neuron, neuron);
                  }
                  sigma(layer, neuron, summation * derived(layer, neuron));
               }
             }, true, false);
            // Update Bias
            walk_in(delegate(int layer, int neuron, int any){
               this.neuron(layer, neuron).bias -= learning_rate * sigma(layer, neuron);
            }, true, false);
            // Update Deltas
            walk_in(delegate(int layer, int neuron, int input){
               if(layer > 0)
                  this.neuron(layer, neuron).delta[input] += sigma(layer, neuron) * last_output(layer-1, input);
            });
         }
         // Update Weights
         walk_in(delegate(int layer, int neuron, int input){
            if(layer > 0)
               this.neuron(layer, neuron).weight[input] -= learning_rate * delta(layer, neuron, input);
         });
      }
      public double getGlobalError(List<double[]> inputs, List<double[]> answers){
         double error = 0;
         for (int i = 0; i < inputs.Count; i++)
         {
            error += getError(this.stimulate(inputs[i]), answers[i]);
         }
         return error;
      }
      public double getError(double[] response, double[] answer)
      {
         double error = 0;
            for (int i = 0; i < response.Length; i++)
            {
                error += 0.5 * Math.Pow(response[i]-answer[i] , 2);
            }
         return error;
      }
      public bool save(string file_path="")
      {
         string path = (file_path != "")?file_path:Model_File_Path;
         return this.serialize(path);
      }
      public bool serialize(string file_path)
      {
         try
         {
            file_path = Path.GetFullPath(file_path);
            if(Path.IsPathRooted(file_path)){
               // List<double[][][]> structure = new List<double[][][]>();
               double[][][][] structure = new double[layers_count][][][];
               for (int i = 0; i < layers_count; i++)
               {
                  structure[i] = new double[neurons_count(i)][][];
               }
               walk_in(delegate(int x, int y, int z){
                  if(z<0){
                     structure[x][y] = new double[2][];
                     structure[x][y][0] = new double[1] {bias(x,y)};
                     structure[x][y][1] = new double[getInputsCount(x,y)];
                  }else{
                     structure[x][y][1][z] = weight(x,y,z);
                  }
               },true);
               System.IO.File.WriteAllText(file_path, JsonSerializer.Serialize(structure));
               return true;
            }else{
               return false;
            }
         }
         catch (System.Exception)
         {
            return false;
         }
      }
      public bool load(string file_path, bool model_auto=false)
      {
         try
         {
            file_path = Path.GetFullPath(file_path);
            if(Path.IsPathRooted(file_path) && File.Exists(file_path)){
               IEnumerator<string> Line = File.ReadLines(file_path).GetEnumerator();
               string JSON_Info = Line.Current;
               // Do stuff
               Line.MoveNext();
               string JSON_Struct = Line.Current;
               double[][][][] structure = JsonSerializer.Deserialize<double[][][][]>(JSON_Struct);

               //Read the File NeuronalNetwork Structure
               int[] NetworkStructure = new int[structure.Length];
               for (int x = 0; x < structure.Length; x++)
               {
                  NetworkStructure[x] = structure[x].Length; 
               }
               //Read custom Struture
               int[] CustomNetwork = new int[layers_count];
               if(!model_auto){
                  for (int x = 0; x < layers_count; x++)
                  {
                     CustomNetwork[x] = neurons_count(x);
                  }
               }else{
                  this.layers_count = NetworkStructure.Length;
                  this.inputs_count = NetworkStructure[0];
                  this.Layer = new List<Layer>();
                  for (int i = 0; i < NetworkStructure.Length; i++)
                  {
                     int inputs_count = (i>0)?NetworkStructure[i-1]:NetworkStructure[i];
                     this.Layer.Add(new Layer(NetworkStructure[i],inputs_count, random));
                  }
               }
               if(model_auto || string.Join(",", NetworkStructure) == string.Join(",", CustomNetwork)){
                  walk_in(delegate(int x, int y, int z){
                     if(z < 0){
                        bias(x,y, structure[x][y][0][0]);
                     }else{
                        weight(x,y,z, structure[x][y][1][z]);
                     }
                  }, true);
               }else{
                  return false;
               }

               return true;
            }else{
               return false;
            }
         }
         catch (System.Exception)
         {
             return false;
         }
      }
      ~MultiLayerPerceptron(){
         if(this.SaveOn_GarbageCollector){
            if(Path.IsPathRooted(this.Model_File_Path)){
               this.serialize(this.Model_File_Path);
            }
         }
      }
   } 
}