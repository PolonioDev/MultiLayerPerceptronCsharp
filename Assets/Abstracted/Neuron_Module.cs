using System;
using System.Collections.Generic;
using NeuronalNetwok.Activation;

namespace NeuronalNetwok
{
    public class Neuron
    {
        public double[] weight;
        public double bias;
        public double[] delta;
        public double sigma;
        public int inputs_count; 
        public ActivationFunction function;
        public Random random;

        public Neuron(int inputs_count, Random random, string function_name="SIGMOID")
        {
            //initialize variables
            weight = new double[inputs_count];
            delta = new double[inputs_count];
            bias = 0;
            sigma = 0;
            function = new ActivationFunction(function_name);
            this.random = random;
            this.inputs_count = inputs_count;

            reset();
        }
        public double stimulate(double[] input)
        {
            double z =  bias;
            for (int i = 0; i < inputs_count; i++)
            {
                z += weight[i] * input[i];
            }
            return activation(z);
        }
        public double activation(double input)
        {
            return function.exec(input);
        }
        public double derivated(double input)
        {
            return function.derive(input);
        }

        // Learn Stuff
        public bool learn(List<double[]> inputs, List<double> answers, double learning_rate=0.2, double max_error_allowed=0.0001)
        {
            int? score = null;
            while (score != inputs.Count)
            {
                score = 0;
                for (int i = 0; i < inputs.Count; i++)
                {
                    learn_this(inputs[i], answers[i], learning_rate);
                }
                score = getScore(inputs, answers);
            }
            return true;
        }
        public bool learn_this(double[] input, double answer, double learning_rate=0.2)
        {
            double error = getError(input, answer);
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] += (learning_rate*error) * input[i];
            }
            bias += learning_rate*error;
            return true;
        }
        public int getScore(List<double[]> inputs, List<double> answers, double max_error_allowed=0.0001)
        {
            int score = 0;
            double error = 0.0;
            for (int i = 0; i < inputs.Count; i++)
            {
                error = getError(inputs[i],answers[i]);
                if(Math.Abs(error) < max_error_allowed){
                    score++;
                }
            }
            return score;
        }
        public double getGlobalError(List<double[]> inputs, List<double> answers)
        {
            double error = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                error += Math.Abs(getError(inputs[i],answers[i]));
            }
            return error;
        }

        public double getError(double[] input, double answers)
        {
            return (answers-stimulate(input));
        }
        public void reset()
        {
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = 2*random.NextDouble()-1;
                delta[i] = 0;
            }
            bias = 2*random.NextDouble()-1;
            sigma = 0;
        }
        public void define_function(string function_name)
        {
            function = new ActivationFunction(function_name);
        }
        public void define_function(ActivationFunction function)
        {
            this.function = function;
        }
    }
}