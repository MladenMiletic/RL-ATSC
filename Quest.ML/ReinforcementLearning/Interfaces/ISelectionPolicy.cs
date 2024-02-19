using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.Interfaces
{
    public interface ISelectionPolicy
    {
        public int SelectAction(double[] QValues, int[]? actionCounts = null);  
    }
}
