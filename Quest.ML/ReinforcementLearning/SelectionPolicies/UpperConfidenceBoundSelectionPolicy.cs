using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Quest.ML.ReinforcementLearning.SelectionPolicies
{
    public class UpperConfidenceBoundSelectionPolicy(double explorationCoefficient) : ISelectionPolicy
    {
        private double explorationCoefficient = explorationCoefficient;
        private int totalSelections = 0;

        public double ExplorationCoefficient
        {
            get
            {
                return explorationCoefficient;
            }
            set
            {
                explorationCoefficient = value;
            }
        }

        public int SelectAction(double[] QValues, int[]? actionCounts)
        {
            if (actionCounts == null)
            {
                throw new ArgumentNullException(nameof(actionCounts) + "Can not be null for Upper Confidence Bound selection policy");
            }
            if (QValues == null || QValues.Length == 0)
            {
                throw new ArgumentException("QValues array is null or empty");
            }

            for (int i = 0; i < QValues.Length; i++)
            {
                if (actionCounts[i] == 0)
                {
                    totalSelections++;
                    return i;
                }
            }
            double[] upperConfidenceBounds = new double[QValues.Length];
        
            for (int i = 0; i < QValues.Length;i++)
            {
                double explorationBonus = Math.Sqrt((2 * Math.Log(totalSelections)) / actionCounts[i]);
                upperConfidenceBounds[i] = QValues[i] + ExplorationCoefficient * explorationBonus;
            }

            totalSelections++;
            return Array.IndexOf(upperConfidenceBounds, upperConfidenceBounds.Max());
        }
    }
}
