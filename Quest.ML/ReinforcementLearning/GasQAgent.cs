using Quest.ML.Clustering.Neural;
using Quest.ML.Extensions;
using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning
{
    public class GasQAgent : IAgent
    {
        public readonly int numberOfActions;
        public readonly GasNetwork gasNetworkStateIdentifier;
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
        public GasQAgent(int numberOfActions, GasNetwork gasNetworkStateIdentifier, ISelectionPolicy selectionPolicy, double learningRateAlpha, double discountFactorGamma)
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
            gasNetworkStateIdentifier.InitializeNetwork();
        }
        public GasQAgent(GasQAgentParameters parameters)
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
            if (gasNetworkStateIdentifier.BestMatchingUnit1 == null)
            {
                throw new Exception("Failed to Compute best matching unit!");
            }
            return gasNetworkStateIdentifier.BestMatchingUnit1.ID;
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

        public void Learn(int previousStateId, int actionId, int currentStateId,double reward)
        {
            QTable[previousStateId][actionId] = QTable[previousStateId][actionId] + learningRateAlpha * (reward + discountFactorGamma * QTable[currentStateId].Max() - QTable[previousStateId][actionId]);
        }
        

        public int SelectAction(int stateID)
        {
            return SelectionPolicy.SelectAction(QTable[stateID], ActivationCounters[stateID]);
        }

        private void HandleNeuronAddition(object? sender, GasNeuron addedNeuron)
        {
            QTable.Add(addedNeuron.ID, InitializeQTableRow());
            ActivationCounters.Add(addedNeuron.ID, InitializeQTableRowActivationCounter());
        }
        private void HandleNeuronDeletion(object? sender, GasNeuron deletedNeuron)
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
    }
}
