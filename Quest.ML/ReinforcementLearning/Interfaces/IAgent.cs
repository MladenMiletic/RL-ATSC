using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.Interfaces
{
    public interface IAgent
    {

        public ISelectionPolicy SelectionPolicy
        {
            get;
        }
        public int SelectAction(int stateId);
        public void Learn(int previousStateId, int actionId, int currentStateId, double reward);
    }
}
