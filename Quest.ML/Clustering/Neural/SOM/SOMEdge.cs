using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.SOM
{
    public class SOMEdge(SOMNeuron source, SOMNeuron target) : GraphEdge<SOMNeuron>(source, target)
    {
    }
}
