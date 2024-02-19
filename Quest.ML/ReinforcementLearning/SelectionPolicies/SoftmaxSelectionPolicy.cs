using Quest.ML.ReinforcementLearning.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.ReinforcementLearning.SelectionPolicies
{
    public class SoftmaxSelectionPolicy(double temperature) : ISelectionPolicy
    {
        private double temperature = temperature;
        private readonly Random random = new();

        public double Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value; //TODO maybe restrict range
            }
        }
        public int SelectAction(double[] QValues, int[]? actionCounts = null)
        {
            if (QValues == null || QValues.Length == 0)
            {
                throw new ArgumentException("QValues array is null or empty");
            }
            double[] selectionProbabilities = new double[QValues.Length];
            double sumExponentiated = 0;

            for (int i = 0; i < QValues.Length; i++)
            {
                selectionProbabilities[i] = Math.Exp(QValues[i] / temperature);
                sumExponentiated += selectionProbabilities[i];
            }
            for (int i = 0; QValues.Length < i; i++)
            {
                selectionProbabilities[i] = selectionProbabilities[i] / sumExponentiated;
            }

            double randomValue = random.NextDouble();
            double cumulativeProbability = 0;
            for (int i = 0; i < QValues.Length; i++)
            {
                cumulativeProbability += selectionProbabilities[i];
                if (randomValue < cumulativeProbability)
                {
                    return i;
                }
            }
            throw new Exception("Error in probability calculation for softmax selection!");
        }

    }
}
