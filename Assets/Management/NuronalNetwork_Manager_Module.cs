using System;
using System.Collections.Generic;
using NeuronalNetwok;

namespace MultiLayerPerceptron
{
    public class NuronalNetwork_Manager : NeuronalNetwork
    {
        public delegate void function(int layer, int neuron, int input);
        public NuronalNetwork_Manager(int[] structure) : base(structure){
            //Contructor
        }
        public void show_model()
        {
            walk_in(delegate(int layer, int neuron, int input){
                if(input < 0){
                if(neuron == 0)
                    Console.WriteLine("=========LAYER=========");
                Console.WriteLine("===== NEURONA({0},{1}) =====", layer, neuron);
                Console.WriteLine("B({0},{1}) = {2}", layer, neuron, bias(layer, neuron));
                }else{
                Console.WriteLine("W({0},{1},{2}) = {3}", layer, neuron, input, weight(layer, neuron, input));
                }
            }, true);
            Console.WriteLine("======================");
        }
        public void walk_in(function callback, bool neurons=false, bool inputs=true)
        {
            for (int layer = this.layers_count-1; layer >=0 ; layer--){
                if(!neurons && !inputs)
                    callback(layer, -1, -1);
                if(neurons || inputs){
                    for (int neuron = 0; neuron < this.Layer[layer].neurons_count; neuron++){
                        if(neurons)
                            callback(layer, neuron, -1);
                        if(inputs){
                            for (int input = 0; input < this.neuron(layer, neuron).inputs_count; input++){
                                callback(layer, neuron, input);
                            }
                        }
                    }
                }
            }
        }
        public double derived(double input){
            return this.Layer[0].Neuron[0].derivated(input);
        }
        public double derived(int layer, int neuron){
            double input = Layer[layer].Last_Stimulate[neuron]; 
            return this.neuron(layer, neuron).derivated(input);
        }
        public int neurons_count(int layer)
        {
            return this.Layer[layer].neurons_count;
        }
        public double last_output(int layer, int neuron)
        {
            return this.Layer[layer].Last_Stimulate[neuron];
        }
        public int getInputsCount(int layer, int neuron)
        {
            return this.neuron(layer, neuron).inputs_count;
        }
        public double weight(int layer,int neuron, int input)
        {
            return this.neuron(layer, neuron).weight[input];
        }
        public void weight(int layer,int neuron, int input, double value)
        {
            this.neuron(layer, neuron).weight[input] = value;
        }
        public double delta(int layer,int neuron, int input)
        {
            return this.neuron(layer, neuron).delta[input];
        }
        public void delta(int layer,int neuron, int input, double value)
        {
            this.neuron(layer, neuron).delta[input] = value;
        }
        public double bias(int layer,int neuron)
        {
            return this.neuron(layer, neuron).bias;
        }
        public void  bias(int layer,int neuron, double value)
        {
            this.neuron(layer, neuron).bias = value;
        }
        public double sigma(int layer,int neuron, double? value = null)
        {
            return this.neuron(layer, neuron).sigma;
        }
        public void sigma(int layer,int neuron, double value)
        {
            this.neuron(layer, neuron).sigma = value;
        }
        public Neuron neuron(int layer,int neuron)
        {
            return this.Layer[layer].Neuron[neuron];
        }
    }
}