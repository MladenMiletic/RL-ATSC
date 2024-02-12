using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNetwork(double learningRate, int inputDimensionality, int neuronMaturationAge, double neuronConnectionDistance, double neuronAdditionDistance, int edgeMaturationAge) : Graph<GasNeuron, GasEdge>
    {

        private double learningRate = learningRate;
        private int inputDimensionality = inputDimensionality;
        private int neuronMaturationAge = neuronMaturationAge;
        private double neuronConnectionDistance = neuronConnectionDistance;
        private double neuronAdditionDistance = neuronAdditionDistance;
        private int neuronNumberLimit; 
        private int edgeMaturationAge = edgeMaturationAge;
        private int lastNeuronID = 0;

        private int bmu1;
        private int bmu2;

        public int BMU1
        {
            get
            {
                return bmu1;
            } 
        }
        public int BMU2
        {
            get
            {
                return bmu1;
            }
        }

        public double LearningRate
        {
            get
            {
                return learningRate;
            }
            set
            {
                learningRate = value;
            }
        }
        public int InputDimensionality
        {
            get
            {
                return inputDimensionality;
            }
            set
            {
                inputDimensionality = value;
            }
        }
        public int NeuronMaturationAge
        {
            get
            {
                return neuronMaturationAge;
            }
            set
            {
                neuronMaturationAge = value;
            }
        }

        public double NeuronConnectionDistance
        {
            get
            {
                return neuronConnectionDistance;
            }
            set
            {
                neuronConnectionDistance = value;
            }
        }
        public double NeuronAdditionDistance
        {
            get
            {
                return neuronAdditionDistance;
            }
            set
            {
                neuronAdditionDistance = value;
            }
        }
        public int NeuronNumberLimit
        {
            get
            {
                return neuronNumberLimit;
            }
            set
            {
                neuronNumberLimit = value;
            }
        }
        public int EdgeMaturationAge
        {
            get
            {
                return edgeMaturationAge;
            }
            set
            {
                edgeMaturationAge = value;
            }
        }

        public void InitializeNetwork()
        {
            GasNeuron initialNeuron1 = new GasNeuron(InputDimensionality);
            GasNeuron initialNeuron2 = new GasNeuron(InputDimensionality);
            initialNeuron1.RandomizeWeights();
            initialNeuron2.RandomizeWeights();
            this.Add(initialNeuron1);
            this.Add(initialNeuron2);

            this.AddEdge(initialNeuron1, initialNeuron2);
        }

        public override void Add(GasNeuron item)
        {
            item.ID = ++lastNeuronID;
            base.Add(item);
        }


        public double Compute(double[] input)
        {
            if (this.Count == 0)
            {
                return -1;
            }
            SortedList<double,int> computeResult = new SortedList<double,int>();
            foreach (GasNeuron neuron in this)
            {
                computeResult.Add(neuron.Compute(input), neuron.ID);
            }
            var bestResult = computeResult.Min();
            bmu1 = bestResult.Value;
            double error = bestResult.Key;
            computeResult.Remove(bmu1);
            bmu2 = computeResult.Min().Value;
            return error;
        }
        public double Learn()
        {

        }

        //TODO
        /* Network parameters and attributes Y
         * Constructor Y
         * Initial network generation
         * Compute - GET BMU, add remove and so on
         * Update weights of neurons
         * 
         */
    }
}
