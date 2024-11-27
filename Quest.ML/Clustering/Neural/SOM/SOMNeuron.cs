using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.SOM
{
    public class SOMNeuron(int inputDimensionality) : Node(-1)
    {
        private double[] weights = new double[inputDimensionality];
        private int activationCounter = 0;
        private double output;
        private bool wasMoved = false;

        public double[] Weights
        {
            get
            {
                return weights;
            }
            private set
            {
                weights = value;
            }
        }
        public int ActivationCounter
        {
            get
            {
                return activationCounter;
            }
            set
            {
                activationCounter = value;
            }
        }
        public bool WasMoved
        {
            get
            {
                return wasMoved;
            }
            set
            {
                wasMoved = value;
            }
        }
        public double Output
        {
            get
            {
                return output;
            }
        }

        public void SetWeights(double[] weights)
        {
            Weights = new double[weights.Length];
            Array.Copy(weights, Weights, weights.Length);
        }

        public double Compute(double[] input)
        {
            if (weights.Length != input.Length)
            {
                throw new ArgumentException("Input must have the same dimensionality as weights of this neuron");
            }
            double sum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                double diff = weights[i] - input[i];
                sum += diff * diff;
            }
            output = Math.Sqrt(sum);
            return output;
        }
        public void IncrementActivationCounter()
        {
            ActivationCounter++;
        }
        public void ResetActivationCounter()
        {
            ActivationCounter = 0;
        }

        public void RandomizeWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = rnd.NextDouble() * 20;
            }
        }

    }
}
