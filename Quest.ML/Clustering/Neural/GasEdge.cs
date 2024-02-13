using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasEdge(GasNeuron source, GasNeuron target) : GraphEdge<GasNeuron>(source, target)
    {
        private int age = 0;

        public int Age
        {
            get
            {
                return age;
            }
            private set
            {
                age = value;
            }
        }

        public GasEdge ResetAge()
        {
            Age = 0;
            return this;
        }
        
    
    }
}
