using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Quest.ML.ReinforcementLearning.SelectionPolicies
{
    public class GreedySelectionPolicy : ISelectionPolicy
    {
        public int SelectAction(double[] QValues, int[]? actionCounts = null)
        {
            if (QValues == null || QValues.Length == 0)
            {
                throw new ArgumentException("QValues array is null or empty");
            }

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
