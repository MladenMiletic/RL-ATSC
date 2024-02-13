using Quest.ML.Clustering.Neural.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GaussianNeighbourhoodFunction : INeighbourhoodFunction
    {
        private double neighbourhoodWidth = 5;
        public GaussianNeighbourhoodFunction()
        {

        }
        public GaussianNeighbourhoodFunction(double neighbourhoodWidth)
        {
            NeighbourhoodWidth = neighbourhoodWidth;
        }

        public double NeighbourhoodWidth
        {
            get => neighbourhoodWidth;
            set => neighbourhoodWidth = value;
        }

        public double Calculate(double distance)
        {
            return Math.Exp(-(distance * distance) / (2*neighbourhoodWidth));
        }
    }
}
