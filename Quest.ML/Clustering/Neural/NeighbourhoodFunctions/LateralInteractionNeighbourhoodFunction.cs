using Quest.ML.Clustering.Neural.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.NeighbourhoodFunctions
{
    public class LateralInteractionNeighbourhoodFunction : INeighbourhoodFunction
    {
        public double Calculate(double distance)
        {
            if (distance <= 1)
            {
                return 1;
            }
            return 0;
        }
    }
}
