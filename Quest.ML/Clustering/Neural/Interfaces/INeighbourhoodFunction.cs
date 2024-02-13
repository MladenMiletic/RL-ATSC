using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.Interfaces
{
    public interface INeighbourhoodFunction
    {
        public double Calculate(double distance);
    }
}
