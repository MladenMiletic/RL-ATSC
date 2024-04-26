using Quest.ML.Clustering.Neural.Interfaces;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNetwork : Graph<GasNeuron, GasEdge>
    {

        private double learningRate;
        private int inputDimensionality;
        private int neuronMaturationAge;
        private double neuronConnectionDistance;
        private double neuronAdditionDistance;
        private int neuronNumberLimit=100; 
        private int edgeMaturationAge;
        private int lastNeuronID = 0;
        public INeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();

        private GasNeuron? bestMatchingUnit1 = null;
        private GasNeuron? bestMatchingUnit2 = null;

        public event EventHandler<GasNeuron>? NeuronAdded;
        public event EventHandler<GasNeuron>? NeuronDeleted;

        public GasNetwork(double learningRate, int inputDimensionality, int neuronMaturationAge, double neuronConnectionDistance, double neuronAdditionDistance, int edgeMaturationAge)
        {
            this.learningRate = learningRate;
            this.inputDimensionality = inputDimensionality;
            this.neuronMaturationAge = neuronMaturationAge;
            this.neuronConnectionDistance = neuronConnectionDistance;
            this.neuronAdditionDistance = neuronAdditionDistance;
            this.edgeMaturationAge = edgeMaturationAge;
        }

        public GasNetwork(GasNetworkParameters parameters)
        {
            this.learningRate = parameters.LearningRate;
            this.inputDimensionality = parameters.InputDimensionality;
            this.neuronMaturationAge = parameters.NeuronMaturationAge;
            this.neuronConnectionDistance = parameters.NeuronConnectionDistance;
            this.neuronAdditionDistance = parameters.NeuronAdditionDistance;
            this.edgeMaturationAge = parameters.EdgeMaturationAge;
            this.neighbourhoodFunction = parameters.neighbourhoodFunction;
        }

        public GasNeuron? BestMatchingUnit1
        {
            get
            {
                return bestMatchingUnit1;
            }
            private set
            {
                bestMatchingUnit1 = value;
            }
        }
        public GasNeuron? BestMatchingUnit2
        {
            get
            {
                return bestMatchingUnit2;
            }
            private set
            {
                bestMatchingUnit2 = value;
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
            OnNeuronAddition(item);
        }

        public override bool Remove(GasNeuron item)
        {
            bool RemoveResult = base.Remove(item);
            if (RemoveResult)
            {
                OnNeuronDeletion(item);
            }
            return RemoveResult;
        }


        public double Compute(double[] input)
        {
            if (this.Count == 0)
            {
                return -1;
            }
            BestMatchingUnit1 = null;
            BestMatchingUnit2 = null;
            foreach (GasNeuron neuron in this)
            {
                double error = neuron.Compute(input);
                if (BestMatchingUnit1 == null)
                {
                    BestMatchingUnit1 = neuron;
                }
                else if (BestMatchingUnit2 == null)
                {
                    if (error < BestMatchingUnit1.Output)
                    {
                        BestMatchingUnit2 = BestMatchingUnit1;
                        BestMatchingUnit1 = neuron;
                    }
                    else
                    {
                        BestMatchingUnit2 = neuron;
                    }
                }
                else if (error < BestMatchingUnit1.Output)
                {
                    BestMatchingUnit2 = BestMatchingUnit1;
                    
                    BestMatchingUnit1 = neuron;
                }
                else if (error < BestMatchingUnit2.Output)
                {
                    BestMatchingUnit2 = neuron;
                }
            }
            if (BestMatchingUnit1 == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            return BestMatchingUnit1.Output;
        }
        public override GasEdge AddEdge(GasNeuron source, GasNeuron target)
        {
            GasEdge newEdge = new GasEdge(source, target);

            var existingEdge = (from e in this.Edges where e.Equals(newEdge) select e).FirstOrDefault();

            if (existingEdge == null)
            {
                return base.AddEdge(source, target);
            }
            
            existingEdge.ResetAge();

            return existingEdge;
        }

        public double Learn(double[] input)
        {
            if (input.Length != this.InputDimensionality)
            {
                throw new ArgumentException("Input must have the same dimensionality as defined is network.InputDimensionality");
            }
            double error = Compute(input);
            if (BestMatchingUnit1 == null || BestMatchingUnit2 == null)
            {
                throw new NullReferenceException("Best matching unit can not be null!");
            }
            BestMatchingUnit1.Activation(NeuronMaturationAge);
            if (error < NeuronConnectionDistance || this.Count >= neuronNumberLimit)
            {
                this.AddEdge(BestMatchingUnit1, BestMatchingUnit2);
            }
            else if (error < NeuronAdditionDistance)
            {
                AddNewNeuronConnectedWithBestMatchingUnit(input);
            }
            else
            {
                AddNewNeuronNotConnectedToOthers(input);
            }
            UpdateNetwork(new List<GasNeuron> { BestMatchingUnit1 }, 0, input);
            return error;
        }

        private void AddNewNeuronNotConnectedToOthers(double[] input)
        {
            GasNeuron newNeuron = new GasNeuron(InputDimensionality);
            this.Add(newNeuron);
            newNeuron.SetWeights(input);
            BestMatchingUnit1 = newNeuron;
        }

        private void AddNewNeuronConnectedWithBestMatchingUnit(double[] input)
        {
            if (BestMatchingUnit1 == null)
            {
                throw new NullReferenceException("Best matching unit 1 can't be null!");
            }
            GasNeuron newNeuron = new GasNeuron(InputDimensionality);
            this.Add(newNeuron);
            newNeuron.SetWeights(input);
            this.AddEdge(newNeuron, BestMatchingUnit1);
            BestMatchingUnit1 = newNeuron;
        }

        private void UpdateNetwork(List<GasNeuron> gasNeuronsForUpdate, int distanceToBestMatchingUnit, double[] input)
        {
            if (gasNeuronsForUpdate.Count == 0)
            {
                return;
            }
            List<GasNeuron> neuronsForFutureUpdate = new List<GasNeuron>();
            foreach (GasNeuron gasNeuron in gasNeuronsForUpdate)
            {
                UpdateNeuron(gasNeuron, distanceToBestMatchingUnit, input);

                foreach (GasNeuron connectedNeuron in gasNeuron.Connections)
                {
                    if (!connectedNeuron.WasMoved)
                    {
                        neuronsForFutureUpdate.Add(connectedNeuron);
                        connectedNeuron.WasMoved = true;
                    }
                }
            }
            UpdateNetwork(neuronsForFutureUpdate, distanceToBestMatchingUnit++, input);
            if (distanceToBestMatchingUnit == 0)
            {
                foreach (GasNeuron neuron in this)
                {
                    neuron.WasMoved = false;
                }
            }
        }

        private void UpdateNeuron(GasNeuron gasNeuron, int distanceToBestMatchingUnit, double[] input)
        {
            double neighborhoodCoefficient = neighbourhoodFunction.Calculate(distanceToBestMatchingUnit);
            
            for (int i = 0; i < InputDimensionality; i++)
            {
                gasNeuron.Weights[i] += neighborhoodCoefficient*LearningRate*(input[i] - gasNeuron.Weights[i]);
            }
        }

        public void IncrementAgeEdges()
        {
            List<GasEdge> edgesForRemoval = new List<GasEdge>();
            foreach (GasEdge edge in this.Edges)
            {
                edge.IncrementAge();
                if (edge.Age > EdgeMaturationAge)
                {
                    edgesForRemoval.Add(edge);
                }
            }
            foreach (GasEdge edge in edgesForRemoval)
            {
                this.RemoveEdge(edge.Source, edge.Target);
            }
        }
        public override void RemoveEdge(GasNeuron source, GasNeuron target)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            GasEdge edge = (GasEdge)Activator.CreateInstance(typeof(GasEdge), new object[] { source, target });
            if (Edges.Contains(edge))
            {
                Edges.Remove(edge);
                source.RemoveConnection(target);
                target.RemoveConnection(source);
                if (target.Connections.Count == 0 && !target.Mature)
                {
                    this.Remove(target);
                }
                if (source.Connections.Count == 0 && !source.Mature)
                {
                    this.Remove(source);
                }
            }
            else
            {
                throw new Exception("Edge does not exist");
            }
        }

        public virtual void OnNeuronDeletion(GasNeuron deletedNeuron)
        {
            NeuronDeleted?.Invoke(this, deletedNeuron);
        }

        public virtual void OnNeuronAddition(GasNeuron addedNeuron)
        {
            NeuronAdded?.Invoke(this, addedNeuron);
        }
    }
}
