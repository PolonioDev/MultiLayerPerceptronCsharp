using System;
using System.Collections.Generic;
using NeuronalNetwok.Activation;

namespace NeuronalNetwok
{
    public class Layer
    {
        public List<Neuron> Neuron;
        public int neurons_count;
        public int inputs_count;
        public Random random;
        private string function_name;
        public double[] Last_Stimulate;
        public Layer(int neurons_count, int inputs_count, Random random, string function_name="SIGMOID")
        {
            this.neurons_count = neurons_count;
            this.inputs_count = inputs_count;
            this.random = random;
            this.function_name = function_name;
            this.Last_Stimulate = new double[neurons_count];
            Neuron = new List<Neuron>();
            for (int i = 0; i < neurons_count; i++)
            {
                Neuron.Add(new Neuron(inputs_count, random, function_name));
                this.Last_Stimulate[i] = 0;
            }
        }
        public void add(int neurons_count)
        {
            for (int i = 0; i < neurons_count; i++)
            {
                Neuron.Add(new Neuron(inputs_count, random, function_name));
            }
            this.neurons_count += neurons_count;
            this.Last_Stimulate = new double[neurons_count];
        }
        public double[] stimulate(double[] inputs)
        {
            Last_Stimulate = new double[neurons_count];
            double[] output = new double[neurons_count];
            for (int i = 0; i < neurons_count; i++)
            {
                output[i] = Neuron[i].stimulate(inputs);
            }
            Last_Stimulate = output;
            return output;
        }
        public void define_function(string function_name)
        {
            for (int i = 0; i < neurons_count; i++)
            {
                Neuron[i].define_function(function_name);
            }
        }
        public void define_function(ActivationFunction function)
        {
            for (int i = 0; i < neurons_count; i++)
            {
                Neuron[i].define_function(function);
            }
        }
        public void reset()
        {
            for (int i = 0; i < neurons_count; i++)
            {
                Neuron[i].reset();
            }
        }
    }
}