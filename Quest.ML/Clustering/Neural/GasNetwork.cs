using System;
using System.Collections;
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

        private GasNeuron? bmu1 = null;
        private GasNeuron? bmu2 = null;

        public GasNeuron BMU1
        {
            get
            {
                return bmu1;
            }
            private set
            {
                bmu1 = value;
            }
        }
        public GasNeuron BMU2
        {
            get
            {
                return bmu2;
            }
            private set
            {
                bmu2 = value;
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
            foreach (GasNeuron neuron in this)
            {
                double error = neuron.Compute(input);
                if (BMU1 == null)
                {
                    BMU1 = neuron;
                }
                else if (BMU2 == null)
                {
                    BMU2 = neuron;
                }
                else if (error < BMU1.Output)
                {
                    BMU2 = BMU1;
                    BMU1 = neuron;
                }
                else if (error < BMU2.Output)
                {
                    BMU2 = neuron;
                }
            }
            return BMU1.Output;

        }
        public double Learn()
        {
            throw new NotImplementedException();
        }
    }
}
