using System;

namespace NeuronalNetwok
{
    namespace Activation
    {
        public class ActivationFunction
        {
            public delegate double func(double input);
            public func function; 
            public func derivated;
            public ActivationFunction(string function_name="EMPTY")
            {
                function_name = function_name.ToUpper();

                switch (function_name)
                {
                    case "EMPTY":
                        // To be noting
                    break;
                    // case "SIGMOID":
                    //     function = new func(sigmoid);
                    //     derivated = new func(sigmoid_derivated);
                    // break;

                    default:
                        function = new func(sigmoid);
                        derivated = new func(sigmoid_derivated);
                    break;
                }
            }
            public double exec(double input)
            {
                return function(input);
            }
            public double derive(double input)
            {
                return derivated(input);
            }

            public void setFunction(func function, func derivated)
            {
                this.function = function;
                this.derivated = derivated;
            }
            protected double sigmoid(double input)
            {
                return 1 / (1 + Math.Exp(-input));
            }
            protected double sigmoid_derivated(double input)
            {
                return input*(1-input);
            }
        }
    }
}