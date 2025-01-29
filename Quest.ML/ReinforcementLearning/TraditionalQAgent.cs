using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.Extensions;
using Quest.ML.ReinforcementLearning.IGNGAgents;
using Quest.ML.ReinforcementLearning.Interfaces;

namespace Quest.ML.ReinforcementLearning
{
    public class TraditionalQAgent : IAgent
    {
        public readonly int numberOfActions;
        private readonly ISelectionPolicy selectionPolicy;

        private double learningRateAlpha;
        private double discountFactorGamma;

        public readonly SortedDictionary<int, double[]> QTable;
        public readonly SortedDictionary<int, int[]> ActivationCounters;
        private int[] binsPerDimension;  // Number of bins for each dimension
        private double[] minBounds;     // Minimum values for each dimension
        private double[] maxBounds;     // Maximum values for each dimension
        private double[] binSizes;      // Bin size for each dimension

        public void StateIdentifierSetup(int[] binsPerDimension, double[] minBounds, double[] maxBounds)
        {
            if (binsPerDimension.Length != minBounds.Length || binsPerDimension.Length != maxBounds.Length)
                throw new ArgumentException("Dimensions of bins, minBounds, and maxBounds must match.");

            this.binsPerDimension = binsPerDimension;
            this.minBounds = minBounds;
            this.maxBounds = maxBounds;
            this.binSizes = new double[binsPerDimension.Length];

            for (int i = 0; i < binsPerDimension.Length; i++)
            {
                this.binSizes[i] = (maxBounds[i] - minBounds[i]) / binsPerDimension[i];
            }
        }

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
        public void RunGNGTrainingEpoch()
        {
            return;
        }
        public TraditionalQAgent(int numberOfActions, int numberOfStates, ISelectionPolicy selectionPolicy, double learningRateAlpha, double discountFactorGamma)
        {
            this.numberOfActions = numberOfActions;
            this.selectionPolicy = selectionPolicy;
            this.learningRateAlpha = learningRateAlpha;
            this.discountFactorGamma = discountFactorGamma;
            QTable = [];
            ActivationCounters = [];
            InitializeQTable(numberOfStates);
            StateIdentifierSetup(new int[] { 2,2,2,2,2,2,2,2 }, new double[] { 0, 0, 0, 0, 0, 0, 0, 0}, new double[] { 173.748 , 323.532 , 308.144 , 285.314, 173.748, 323.532, 308.144, 285.314 });
            
        }

        public int GetStateId(double[] inputs)
        {
            if (inputs.Length != binsPerDimension.Length)
                throw new ArgumentException("Input dimension must match the state space dimension.");

            int stateId = 0;
            int multiplier = 1;

            for (int i = 0; i < inputs.Length; i++)
            {
                // Clamp input within bounds
                double clampedValue = Math.Max(minBounds[i], Math.Min(inputs[i], maxBounds[i]));

                // Determine the bin index for this dimension
                int binIndex = (int)((clampedValue - minBounds[i]) / binSizes[i]);
                binIndex = Math.Min(binIndex, binsPerDimension[i] - 1); // Ensure within range

                // Map to a unique ID
                stateId += binIndex * multiplier;
                multiplier *= binsPerDimension[i];
            }

            return stateId;
        }


        public void Learn(int previousStateId, int actionId, int currentStateId, double reward)
        {
            QTable[previousStateId][actionId] = QTable[previousStateId][actionId] + learningRateAlpha * (reward + discountFactorGamma * QTable[currentStateId].Max() - QTable[previousStateId][actionId]);
        }


        public int SelectAction(int stateID)
        {
            return SelectionPolicy.SelectAction(QTable[stateID], ActivationCounters[stateID]);
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
        private void InitializeQTable(int numberOfStates)
        {
            for (int i = 0; i < numberOfStates; i++)
            {
                QTable.Add(i, InitializeQTableRow());
                ActivationCounters.Add(i, InitializeQTableRowActivationCounter());
            }
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
