using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.IGNGAgents
{
    public class IGNGQAgentParameters
    {
        public int numberOfActions;
        public IGNGNetwork gasNetworkStateIdentifier;
        private ISelectionPolicy selectionPolicy;

        private double learningRateAlpha = 0.1;
        private double discountFactorGamma = 0.8;

        public IGNGQAgentParameters(int numberOfActions, IGNGNetwork gasNetworkStateIdentifier, ISelectionPolicy selectionPolicy)
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
