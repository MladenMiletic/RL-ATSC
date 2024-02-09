using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNeuron : Node
    {
        private double[] weights;
        private int age;
        private int activationCounter;
        private double output;

        public GasNeuron(int id) : base(id)
        {
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
            private set
            {
                age = value;
            }
        }
        public int ActivationCounter
        {
            get
            {
                return activationCounter;
            }
        }
    }
}
