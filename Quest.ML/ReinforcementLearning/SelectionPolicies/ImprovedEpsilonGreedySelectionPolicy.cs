using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.SelectionPolicies
{
    public class ImprovedEpsilonGreedySelectionPolicy : ISelectionPolicy
    {
        private double epsilon = 1;
        private readonly Random random = new();

        public double Epsilon
        {
            get
            {
                return epsilon;
            }
            set
            {
                if (value > 1 || value < 0)
                {
                    throw new ArgumentOutOfRangeException("Epsilon must be in range from 0 to 1");
                }
                epsilon = value;
            }
        }

        public int SelectAction(double[] QValues, int[]? actionCounts = null)
        {
            if (QValues == null || QValues.Length == 0)
            {
                throw new ArgumentException("QValues array is null or empty");
            }

            double generatedValue = random.NextDouble();
            if (generatedValue < epsilon) //Explore
            {
                return random.Next(0, QValues.Length);
            }
            else if (QValues.Contains(0)) // Adapt
            {
                return Array.IndexOf(QValues, 0);
            }
            else //Exploit
            {
                int maxIndex = 0;
                for (int i = 1; i < QValues.Length; i++)
                {
                    if (QValues[i] > QValues[maxIndex])
                    {
                        maxIndex = i;
                    }
                }
                return maxIndex;
            }

        }
    }
}
