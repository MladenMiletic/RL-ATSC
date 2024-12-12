using Quest.ML.Clustering.Neural.SOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.GNG
{
    public class GNGEdge(GNGNeuron source, GNGNeuron target) : GraphEdge<GNGNeuron>(source, target)
    {
        public int age = 0;
    }
}
