﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasEdge : GraphEdge<GasNeuron>
    {
        public GasEdge(GasNeuron source, GasNeuron target) : base(source, target)
        {
        }
    }
}
