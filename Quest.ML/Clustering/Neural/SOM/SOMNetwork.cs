using Quest.ML.Clustering.Neural.Interfaces;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.SOM
{
    public class SOMNetwork : Graph<SOMNeuron, SOMEdge>
    {
        private int inputDimensionality;
        private int numberOfNeurons;
        public INeighbourhoodFunction neighbourhoodFunction = new LateralInteractionNeighbourhoodFunction();

        private SOMNeuron BMU = null;
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

        public SOMNetwork(int inputDimensionality, int numberOfNeurons)
        {
            this.inputDimensionality = inputDimensionality;
            this.numberOfNeurons = numberOfNeurons;
            InitializeNetwork();
        }

        public void InitializeNetwork()
        {
            int width = GetWidth();
            int height = numberOfNeurons / width;
            for (int i = 1; i <= numberOfNeurons; i++)
            {
                SOMNeuron neuron = new SOMNeuron(this.inputDimensionality);
                neuron.RandomizeWeights();
                neuron.ID = i;
                base.Add(neuron);
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width-1; j++)
                {
                    this.AddEdge(this.Nodes[i*width + j], this.Nodes[i * width + j + 1]);
                }
            }
            for (int i = 0; i < height-1; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    this.AddEdge(this.Nodes[i * width + j], this.Nodes[i * width + j + width]);
                }
            }

        }

        private int GetWidth()
        {
            int width = 1;
            for (int i = 2; i <= Math.Sqrt(numberOfNeurons); i++)
            {
                if (numberOfNeurons % i == 0)
                {
                    width = i;
                }
            }
            return width;
        }
        public double Compute(double[] input)
        {
            if (this.Count == 0)
            {
                return -1;
            }
            BMU = null;
            foreach (SOMNeuron neuron in this)
            {
                double error = neuron.Compute(input);
                if (BMU == null)
                {
                    BMU = neuron;
                }
                else if (error < BMU.Output)
                {
                    BMU = neuron;
                }
            }
            if (BMU == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            return BMU.Output;
        }
        public double Learn(double[] input)
        {
            if (input.Length != this.InputDimensionality)
            {
                throw new ArgumentException("Input must have the same dimensionality as defined is network.InputDimensionality");
            }
            double error = Compute(input);
            if (BMU == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            BMU.IncrementActivationCounter();
            
            UpdateNetwork(new List<SOMNeuron> { BMU }, 0, input);
            return error;
        }
        private void UpdateNetwork(List<SOMNeuron> somNeuronsForUpdate, int distanceToBestMatchingUnit, double[] input)
        {
            if (somNeuronsForUpdate.Count == 0)
            {
                return;
            }
            List<SOMNeuron> neuronsForFutureUpdate = new List<SOMNeuron>();
            foreach (SOMNeuron somNeuron in somNeuronsForUpdate)
            {
                UpdateNeuron(somNeuron, distanceToBestMatchingUnit, input);

                foreach (SOMNeuron connectedNeuron in somNeuron.Connections)
                {
                    if (!connectedNeuron.WasMoved)
                    {
                        neuronsForFutureUpdate.Add(connectedNeuron);
                        connectedNeuron.WasMoved = true;
                    }
                }
            }
            UpdateNetwork(neuronsForFutureUpdate, ++distanceToBestMatchingUnit, input);
            if (distanceToBestMatchingUnit == 0)
            {
                foreach (SOMNeuron neuron in this)
                {
                    neuron.WasMoved = false;
                }
            }
        }

        private void UpdateNeuron(SOMNeuron somNeuron, int distanceToBestMatchingUnit, double[] input)
        {
            double neighborhoodCoefficient = neighbourhoodFunction.Calculate(distanceToBestMatchingUnit);
            if (somNeuron.ActivationCounter == 0)
            {
                //for (int i = 0; i < InputDimensionality; i++)
                //{
                //    somNeuron.Weights[i] += neighborhoodCoefficient  * (input[i] - somNeuron.Weights[i]);
                //}
                return;
            }
            for (int i = 0; i < InputDimensionality; i++)
            {
                somNeuron.Weights[i] += neighborhoodCoefficient * (1/(double) somNeuron.ActivationCounter) * (input[i] - somNeuron.Weights[i]);
            }
        }
        public void AppendRMSE(string filePath, double rmse)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(rmse); // Append the RMSE value as a new row
            }
        }

        public double CalculateRMSE(List<double[]> dataList)
        {
            double sumSqErrors = 0;
            foreach (double[] data in dataList)
            {
                sumSqErrors += Math.Pow(this.Compute(data),2);
            }
            double mse = sumSqErrors / dataList.Count;
            return Math.Sqrt(mse);
        }
    }
}
