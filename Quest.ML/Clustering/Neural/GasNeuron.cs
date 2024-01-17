using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNeuron
    {
		private int id;
		private double[] weights;
        private int age;
		private int activationCounter;
		private double output;

        public int ID
		{
			get
			{
				return id;
			}
			private set
			{
				id = value;
			}
		}
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
		public int Age
		{
			get
			{
				return age;
			}
			set
			{
				age = value;
			}
		}
		public double Output
		{
            get
            {
                return output;
            }
            private set
            {
                output = value;
            }
        }

		public GasNeuron(int id, int numInputDimensions)
		{
			this.id = id;
			weights = new double[numInputDimensions];
			age = 0;
			activationCounter = 0;
		}

		public double Compute(double[] inputs)
		{
			double sumOfSquares = 0;

			for (int i = 0; i < weights.Length; i++)
			{
                sumOfSquares += Math.Pow(weights[i] - inputs[i], 2);
            }
			output = Math.Sqrt(sumOfSquares);
			return output;
		}

		public void UpdateWeights(double[] weights)
		{
			Array.Copy(weights, this.weights, weights.Length);
        }

		public void IncrementAge()
		{
			age++;
		}
		public void ResetAge()
		{
			age = 0;
		}


	}
}
