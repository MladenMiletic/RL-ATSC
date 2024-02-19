using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.Interfaces
{
    public interface IAgent
    {
        public void SelectAction();
        public void TransitionStep();
        public void Learn(double reward);
    }
}
