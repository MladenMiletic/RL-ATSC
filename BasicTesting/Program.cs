using Quest.ML.Clustering.Neural;
using System.Diagnostics;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            DistanceMatrix distanceMatrix = new DistanceMatrix();

            distanceMatrix.AddRow(1);  //Dodaje čvor koji nije povezan
            distanceMatrix.AddRow(2, 1); //Dodaje čvor 2 koji će se povezati s 1
            distanceMatrix.AddRow(3, 2); //Dodaje čvor 3 koji će se povezati s 2
            distanceMatrix.AddRow(4, 3); //Dodaje čvor 4 koji će se povezati s 3
            distanceMatrix.AddRow(5, 4); //Dodaje čvor 5 koji će se povezati s 4
            distanceMatrix.AddRow(6, 5); //Dodaje čvor 6 koji će se povezati s 5
            Console.WriteLine(distanceMatrix);
            Console.WriteLine();
            distanceMatrix.UpdateAfterEdgeAdded(6, 2); //Dodaje edge izmedu 6 i 2

            Console.WriteLine(distanceMatrix);

            Console.ReadKey();
        }

    }
}
