using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.SelectionPolicies
{
    public class RandomSelectionPolicy : ISelectionPolicy
    {
        private readonly Random random = new();
        public int SelectAction(double[] QValues, int[]? actionCounts = null)
        {
            return random.Next(QValues.Length);
        }
    }
}
