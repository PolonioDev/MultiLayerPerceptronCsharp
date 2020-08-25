using System;
using System.Collections.Generic;
using NeuronalNetwok.Activation;

namespace NeuronalNetwok
{
    public class NeuronalNetwork
    {
        public List<Layer> Layer;
        public  int inputs_count;
        public Random random;
        public int layers_count;
        public NeuronalNetwork(int[] structure)
        {
            this.Layer = new List<Layer>();
            this.random = new Random(Guid.NewGuid().GetHashCode());
            this.inputs_count = structure[0];
            this.layers_count = structure.Length;
            for (int i = 0; i < structure.Length; i++)
            {
                int neurons_count = structure[i];
                int inputs_count = i == 0 ? structure[i] : structure[i-1];
                this.Layer.Add(new Layer(neurons_count, inputs_count, random));  
            }
        }
        public double[] stimulate(double[] input)
        {
            double[] output = new double[Layer[0].neurons_count];
            for (int i = 0; i < layers_count; i++)
            {
                output = Layer[i].stimulate(input);
                input = output;
            }
            return output;
        }

        public void define_function(string function_name)
        {
            for (int i = 0; i < layers_count; i++)
            {
                Layer[i].define_function(function_name);
            }
        }
        public void define_function(ActivationFunction function)
        {
            for (int i = 0; i < layers_count; i++)
            {
                Layer[i].define_function(function);
            }
        }
        public void reset()
        {
            for (int i = 0; i < layers_count; i++)
            {
                Layer[i].reset();
            }
        }
    }
}