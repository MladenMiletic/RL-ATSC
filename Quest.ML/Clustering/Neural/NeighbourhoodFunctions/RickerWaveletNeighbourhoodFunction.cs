using Quest.ML.Clustering.Neural.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural.NeighbourhoodFunctions
{
    /// <summary>
    /// Ricker wavelet is sometimes called "Mexican hat function" which is culturaly insensitive,
    /// hence the proper name is used here
    /// </summary>
    public class RickerWaveletNeighbourhoodFunction : INeighbourhoodFunction
    {
        private double neighbourhoodWidth = 5;
        public RickerWaveletNeighbourhoodFunction()
        {

        }
        public RickerWaveletNeighbourhoodFunction(double neighbourhoodWidth)
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
            double distanceSquared = distance * distance;
            return (1 - distanceSquared / NeighbourhoodWidth) * Math.Exp(-distanceSquared / (2 * neighbourhoodWidth));
        }
    }
}
