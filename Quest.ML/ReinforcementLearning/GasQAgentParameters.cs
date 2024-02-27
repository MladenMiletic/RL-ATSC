using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning
{
    //int numberOfActions,
    //GasNetwork gasNetworkStateIdentifier,
    //ISelectionPolicy selectionPolicy,
    //double learningRateAlpha,
    //double discountFactorGamma
    public class GasQAgentParameters
    {
        public int numberOfActions;
        public GasNetwork gasNetworkStateIdentifier;
        private ISelectionPolicy selectionPolicy;

        private double learningRateAlpha = 0.1;
        private double discountFactorGamma = 0.8;

        public GasQAgentParameters(int numberOfActions, GasNetwork gasNetworkStateIdentifier, ISelectionPolicy selectionPolicy)
        {
            this.numberOfActions = numberOfActions;
            this.gasNetworkStateIdentifier = gasNetworkStateIdentifier;
            this.selectionPolicy = selectionPolicy;
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
    }
}
