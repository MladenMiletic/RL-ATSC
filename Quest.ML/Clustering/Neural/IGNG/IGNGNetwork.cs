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
    public class IGNGNetwork : Graph<IGNGNeuron, IGNGEdge>
    {
        private int inputDimensionality;
        private int numberOfNeurons;
        private int maxNeurons;
        public int maxAge = 7;
        public int maturationAge = 5;
        public double growingDistance = 80;
        public int lastNodeID = 2;
        public INeighbourhoodFunction neighbourhoodFunction = new LateralInteractionNeighbourhoodFunction();

        public IGNGNeuron BMU1 = null;
        public IGNGNeuron BMU2 = null;

        public event EventHandler<IGNGNeuron>? NeuronAdded;
        public event EventHandler<IGNGNeuron>? NeuronDeleted;
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

        public IGNGNetwork(int inputDimensionality, int numberOfNeurons)
        {
            this.inputDimensionality = inputDimensionality;
            this.maxNeurons = numberOfNeurons;
            //InitializeNetwork();
        }

        public override void Add(IGNGNeuron item)
        {
            item.ID = ++lastNodeID;
            base.Add(item);
            OnNeuronAddition(item);
        }

        public override bool Remove(IGNGNeuron item)
        {
            bool RemoveResult = base.Remove(item);
            if (RemoveResult)
            {
                OnNeuronDeletion(item);
            }
            return RemoveResult;
        }

        public void InitializeNetwork()
        {
            IGNGNeuron neuron1 = new IGNGNeuron(this.inputDimensionality);
            neuron1.RandomizeWeights();
            neuron1.ID = 1;
            IGNGNeuron neuron2 = new IGNGNeuron(this.inputDimensionality);
            neuron2.RandomizeWeights();
            neuron2.ID = 2;
            this.Add(neuron1);
            this.Add(neuron2);
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
            foreach (IGNGNeuron neuron in this)
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
            if (BMU1.Output >= growingDistance)
            {
                if (this.Nodes.Count < this.maxNeurons)
                {
                    //ADD embryo with input as weights
                    IGNGNeuron embryo = new IGNGNeuron(this.InputDimensionality);
                    embryo.SetWeights(input);
                    this.Add(embryo);
                    embryo.ID = lastNodeID + 1;
                    lastNodeID = embryo.ID;
                    BMU1 = embryo;
                }
                BMU1.IncrementAgeCounter();
            }
            else
            {
                if (BMU2.Output >= growingDistance)
                {
                    if (this.Nodes.Count < this.maxNeurons)
                    {
                        //ADD embryo with input as weights
                        IGNGNeuron embryo = new IGNGNeuron(this.InputDimensionality);
                        embryo.SetWeights(input);
                        this.Add(embryo);
                        AddEdge(embryo, BMU1);
                        embryo.ID = lastNodeID + 1;
                        lastNodeID = embryo.ID;
                        BMU1 = embryo;
                    }
                    BMU1.IncrementAgeCounter();
                }
                else
                {
                    foreach (IGNGEdge edge in this.Edges)
                    {
                        if (edge.Source == BMU1 || edge.Target == BMU1)
                        {
                            edge.age += 1;
                        }
                        
                    }
                    UpdateNeuron(BMU1, 0, input);
                    BMU1.IncrementAgeCounter();
                    foreach (IGNGNeuron neuron in BMU1.Connections)
                    {
                        UpdateNeuron(neuron, 1, input);
                        neuron.IncrementAgeCounter();
                    }
                    AddEdge(BMU1, BMU2);
                    List<IGNGEdge> edgesToDelete = new List<IGNGEdge>();
                    foreach (IGNGEdge edge in this.Edges)
                    {
                        if (edge.age > this.maxAge)
                        {
                            edgesToDelete.Add(edge);
                        }
                    }
                    foreach (IGNGEdge edge in edgesToDelete)
                    {
                        RemoveEdge(edge.Source, edge.Target);
                    }
                }
            }


            
            return error;
        }

        public override IGNGEdge AddEdge(IGNGNeuron source, IGNGNeuron target)
        {
            IGNGEdge newEdge = new IGNGEdge(source, target);

            var existingEdge = (from e in this.Edges where e.Equals(newEdge) select e).FirstOrDefault();

            if (existingEdge == null)
            {
                return base.AddEdge(source, target);
            }

            existingEdge.age = 0;

            return existingEdge;
        }



        private void UpdateNeuron(IGNGNeuron somNeuron, int distanceToBestMatchingUnit, double[] input)
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
        public override void RemoveEdge(IGNGNeuron source, IGNGNeuron target)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            IGNGEdge edge = (IGNGEdge)Activator.CreateInstance(typeof(IGNGEdge), new object[] { source, target });
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

        public virtual void OnNeuronDeletion(IGNGNeuron deletedNeuron)
        {
            NeuronDeleted?.Invoke(this, deletedNeuron);
        }

        public virtual void OnNeuronAddition(IGNGNeuron addedNeuron)
        {
            NeuronAdded?.Invoke(this, addedNeuron);
        }
    }
}
