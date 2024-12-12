using Quest.ML.Clustering.Neural.Interfaces;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using Quest.ML.Clustering.Neural.SOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.GNG
{
    public class GNGNetwork : Graph<GNGNeuron, GNGEdge>
    {
        private int inputDimensionality;
        private int numberOfNeurons;
        public double errorDecay = 0.98;
        public double errorUpdate = 0.5;
        private int networkUpdate = 100;
        private int maxNeurons;
        public int maxAge = 9;
        public int lastNodeID = 2;
        public INeighbourhoodFunction neighbourhoodFunction = new LateralInteractionNeighbourhoodFunction();

        private GNGNeuron BMU1 = null;
        private GNGNeuron BMU2 = null;
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

        public GNGNetwork(int inputDimensionality, int numberOfNeurons)
        {
            this.inputDimensionality = inputDimensionality;
            this.maxNeurons = numberOfNeurons;
            InitializeNetwork();
        }

        public void InitializeNetwork()
        {
            GNGNeuron neuron1 = new GNGNeuron(this.inputDimensionality);
            neuron1.RandomizeWeights();
            neuron1.ID = 1;
            GNGNeuron neuron2 = new GNGNeuron(this.inputDimensionality);
            neuron2.RandomizeWeights();
            neuron2.ID = 2;
            base.Add(neuron1);
            base.Add(neuron2);
            this.AddEdge(neuron1, neuron2);


        }

        public double Compute(double[] input)
        {
            if (this.Count == 0)
            {
                return -1;
            }
            BMU1 = null;
            BMU2 = null;
            foreach (GNGNeuron neuron in this)
            {
                double error = neuron.Compute(input);
                if (BMU1 == null)
                {
                    BMU1 = neuron;
                }
                else if (BMU2 == null)
                {
                    if (error < BMU1.Output)
                    {
                        BMU2 = BMU1;
                        BMU1 = neuron;
                    }
                    else
                    {
                        BMU2 = neuron;
                    }
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
            if (BMU1 == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            return BMU1.Output;
        }
        public double Learn(double[] input)
        {
            if (input.Length != this.InputDimensionality)
            {
                throw new ArgumentException("Input must have the same dimensionality as defined is network.InputDimensionality");
            }
            double error = Compute(input);
            if (BMU1 == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            BMU1.IncrementActivationCounter();
            foreach (GNGEdge edge in this.Edges)
            {
                if (edge.Source == BMU1 || edge.Target == BMU1)
                {
                    edge.age += 1;
                }
            }
            BMU1.Error += Math.Abs(error);
            UpdateNeuron(BMU1,0,input);
            foreach (GNGNeuron neuron in BMU1.Connections)
            {
                UpdateNeuron(neuron,1,input);
            }
            AddEdge(BMU1, BMU2);
            List<GNGEdge> edgesToDelete = new List<GNGEdge>();
            foreach (GNGEdge edge in this.Edges)
            {
                if (edge.age > this.maxAge)
                {
                    edgesToDelete.Add(edge);
                }
            }
            foreach (GNGEdge edge in edgesToDelete)
            {
                RemoveEdge(edge.Source, edge.Target);
            }
            


            
            return error;
        }

        public override GNGEdge AddEdge(GNGNeuron source, GNGNeuron target)
        {
            GNGEdge newEdge = new GNGEdge(source, target);

            var existingEdge = (from e in this.Edges where e.Equals(newEdge) select e).FirstOrDefault();

            if (existingEdge == null)
            {
                return base.AddEdge(source, target);
            }

            existingEdge.age = 0;

            return existingEdge;
        }



        private void UpdateNeuron(GNGNeuron somNeuron, int distanceToBestMatchingUnit, double[] input)
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
                somNeuron.Weights[i] += neighborhoodCoefficient * (1 / ((double)somNeuron.ActivationCounter)) * (input[i] - somNeuron.Weights[i]);
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
                sumSqErrors += Math.Pow(this.Compute(data), 2);
            }
            double mse = sumSqErrors / dataList.Count;
            return Math.Sqrt(mse);
        }
        public override void RemoveEdge(GNGNeuron source, GNGNeuron target)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            GNGEdge edge = (GNGEdge)Activator.CreateInstance(typeof(GNGEdge), new object[] { source, target });
            if (Edges.Contains(edge))
            {
                Edges.Remove(edge);
                source.RemoveConnection(target);
                target.RemoveConnection(source);
                if (target.Connections.Count == 0)
                {
                    this.Remove(target);
                }
                if (source.Connections.Count == 0)
                {
                    this.Remove(source);
                }
            }
            else
            {
                throw new Exception("Edge does not exist");
            }
        }

        public void Update()
        {
            if (this.Nodes.Count >= this.maxNeurons)
            {
                return; 
            }
            GNGNeuron highestError = null;
            foreach (GNGNeuron neuron in this.Nodes)
            {
                if (highestError == null)
                {
                    highestError = neuron;
                }
                else
                {
                    if (neuron.Error > highestError.Error)
                    {
                        highestError = neuron;
                    }
                }
            }
            GNGNeuron highestConnection = null;
            foreach (GNGNeuron connection in highestError.Connections)
            {
                if (highestConnection == null)
                {
                    highestConnection = connection;
                }
                else
                {
                    if (connection.Error > highestConnection.Error)
                    {
                        highestConnection = connection;
                    }
                }
            }
            GNGNeuron addition = new GNGNeuron(InputDimensionality);
            double[] newWeigths = new double[InputDimensionality];
            for (int i = 0; i < newWeigths.Length; i++)
            {
                newWeigths[i] = 0.5 * (highestConnection.Weights[i] + highestError.Weights[i]);
            }
            addition.SetWeights(newWeigths);
            addition.ID = lastNodeID + 1;
            lastNodeID = addition.ID;
            this.Add(addition);

            AddEdge(addition, highestError);
            AddEdge(addition, highestConnection);
            RemoveEdge(highestConnection, highestError);
            highestError.Error = errorUpdate * highestError.Error;
            highestConnection.Error = errorUpdate * highestConnection.Error;
            addition.Error = highestError.Error;
        }

        public int[] GetLabels(List<double[]> dataList)
        {
            int[] labels = new int[dataList.Count];
            for (int i = 0; i < labels.Length; i++)
            {
                this.Compute(dataList[i]);
                labels[i] = this.BMU1.ID;
            }
            return labels;
        }
    }
}
