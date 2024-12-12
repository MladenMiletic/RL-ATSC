using Quest.ML.Clustering.Neural.SOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.GNG
{
    public class IGNGEdge(IGNGNeuron source, IGNGNeuron target) : GraphEdge<IGNGNeuron>(source, target)
    {
        public int age = 0;
    }
}
