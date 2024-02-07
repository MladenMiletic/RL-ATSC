using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNeuron : Node
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
