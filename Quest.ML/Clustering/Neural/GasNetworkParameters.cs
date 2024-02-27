using Quest.ML.Clustering.Neural.Interfaces;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNetworkParameters
    {
        private double learningRate = 0.1;
        private int inputDimensionality = 3;
        private int neuronMaturationAge = 20;
        private double neuronConnectionDistance = 1;
        private double neuronAdditionDistance = 2;
        private int neuronNumberLimit = 100;
        private int edgeMaturationAge = 10;
        public INeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();

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
    }
}
