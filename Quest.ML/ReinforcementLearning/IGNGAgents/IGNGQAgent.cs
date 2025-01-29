using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.Extensions;
using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.IGNGAgents
{
    public class IGNGQAgent : IAgent
    {
        public readonly int numberOfActions;
        public readonly IGNGNetwork gasNetworkStateIdentifier;
        private readonly ISelectionPolicy selectionPolicy;

        private double learningRateAlpha;
        private double discountFactorGamma;

        public readonly SortedDictionary<int, double[]> QTable;
        public readonly SortedDictionary<int, int[]> ActivationCounters;

        public List<double[]> GNGinputs = new();
        public double GNGerror;

        public ISelectionPolicy SelectionPolicy
        {
            get => selectionPolicy;
        }
        public double LearningRateAlpha
        {
            get => learningRateAlpha;
            set => learningRateAlpha = value;
        }
        public double DiscountFactorGamma
        {
            get => discountFactorGamma;
            set => discountFactorGamma = value;
        }
        public IGNGQAgent(int numberOfActions, IGNGNetwork gasNetworkStateIdentifier, ISelectionPolicy selectionPolicy, double learningRateAlpha, double discountFactorGamma)
        {
            this.numberOfActions = numberOfActions;
            this.gasNetworkStateIdentifier = gasNetworkStateIdentifier;
            this.selectionPolicy = selectionPolicy;
            this.learningRateAlpha = learningRateAlpha;
            this.discountFactorGamma = discountFactorGamma;
            gasNetworkStateIdentifier.NeuronAdded += HandleNeuronAddition;
            gasNetworkStateIdentifier.NeuronDeleted += HandleNeuronDeletion;
            QTable = [];
            ActivationCounters = [];
            //gasNetworkStateIdentifier.InitializeNetwork();
        }
        public IGNGQAgent(IGNGQAgentParameters parameters)
        {
            this.numberOfActions = parameters.numberOfActions;
            this.gasNetworkStateIdentifier = parameters.gasNetworkStateIdentifier;
            this.selectionPolicy = parameters.SelectionPolicy;
            this.learningRateAlpha = parameters.LearningRateAlpha;
            this.discountFactorGamma = parameters.DiscountFactorGamma;
            gasNetworkStateIdentifier.NeuronAdded += HandleNeuronAddition;
            gasNetworkStateIdentifier.NeuronDeleted += HandleNeuronDeletion;
            QTable = [];
            ActivationCounters = [];
            gasNetworkStateIdentifier.InitializeNetwork();
        }

        public int GetStateId(double[] inputs)
        {
            GNGinputs.Add(inputs);
            gasNetworkStateIdentifier.Compute(inputs); //HERE I DID MASSIVE CHANGE
            if (gasNetworkStateIdentifier.BMU1 == null)
            {
                throw new Exception("Failed to Compute best matching unit!");
            }
            return gasNetworkStateIdentifier.BMU1.ID;
        }

        public double RunGNGTrainingEpoch()
        {
            double error = 0;
            GNGinputs.Shuffle();
            foreach (var input in GNGinputs)
            {
                error += gasNetworkStateIdentifier.Learn(input);
            }
            GNGinputs.Clear();
            GNGerror = error;
            return error;
        }

        public void Learn(int previousStateId, int actionId, int currentStateId, double reward)
        {
            QTable[previousStateId][actionId] = QTable[previousStateId][actionId] + learningRateAlpha * (reward + discountFactorGamma * QTable[currentStateId].Max() - QTable[previousStateId][actionId]);
        }


        public int SelectAction(int stateID)
        {
            return SelectionPolicy.SelectAction(QTable[stateID], ActivationCounters[stateID]);
        }
        public int SelectAction(int stateID, bool greedy)
        {
            return SelectionPolicy.SelectAction(QTable[stateID], null);
        }

        private void HandleNeuronAddition(object? sender, IGNGNeuron addedNeuron)
        {
            QTable.Add(addedNeuron.ID, InitializeQTableRow());
            ActivationCounters.Add(addedNeuron.ID, InitializeQTableRowActivationCounter());
        }
        private void HandleNeuronDeletion(object? sender, IGNGNeuron deletedNeuron)
        {
            QTable.Remove(deletedNeuron.ID);
            ActivationCounters.Remove(deletedNeuron.ID);
        }

        private double[] InitializeQTableRow()
        {
            return new double[numberOfActions];
        }
        private int[] InitializeQTableRowActivationCounter()
        {
            return new int[numberOfActions];
        }

        public void SaveQTable(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var entry in QTable)
                {
                    // Combine the key and the values into a single CSV row
                    var line = $"{entry.Key},{string.Join(",", entry.Value)}";
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine($"QTable has been saved to {filePath}");
        }
        public void ReadQTable(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var key = int.Parse(values[0]);
                    var qValues = values.Skip(1).Select(double.Parse).ToArray();
                    QTable[key] = qValues;
                }
            }
            gasNetworkStateIdentifier.NeuronAdded -= HandleNeuronAddition;
            gasNetworkStateIdentifier.NeuronDeleted -= HandleNeuronDeletion;
            Console.WriteLine($"QTable has been read from {filePath}");
        }
    }
}
