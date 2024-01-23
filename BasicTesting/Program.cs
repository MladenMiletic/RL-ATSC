using Quest.ML.Clustering.Neural;
using System.Diagnostics;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            DistanceMatrix distanceMatrix = new DistanceMatrix();

            distanceMatrix.AddRow(1);
            distanceMatrix.AddRow(2, 1);
            distanceMatrix.AddRow(3, 2);
            distanceMatrix.AddRow(4, 3);
            distanceMatrix.AddRow(5, 3);
            distanceMatrix.AddRow(6, 5);
            distanceMatrix.AddRow(7);

            Console.WriteLine(distanceMatrix);
            Console.WriteLine();
            distanceMatrix.ResetReached();
            distanceMatrix.UpdateAfterEdgeAdded(1, 4, 1);

            Console.WriteLine(distanceMatrix);
            Console.WriteLine();
            distanceMatrix.ResetReached();
            distanceMatrix.UpdateAfterEdgeAdded(6, 7, 1);
            Console.WriteLine(distanceMatrix);

            Console.WriteLine();
            distanceMatrix.ResetReached();
            distanceMatrix.UpdateAfterEdgeAdded(3, 7, 1);
            Console.WriteLine(distanceMatrix);
            

            //BIG TEST
            Console.WriteLine("Press key to start big test!");
            Console.ReadKey();
            distanceMatrix = new DistanceMatrix();
            int numNodes = 100;
            for (int i = 1; i <= numNodes; i++)
            {
                distanceMatrix.AddRow(i);
            }

            //RANDOM EDGES
            Random rand = new Random();
            int numEdges = 1000;
            for (int i = 0; i < numEdges; i++)
            {
                distanceMatrix.ResetReached();
                int source = rand.Next(1, numNodes + 1);
                int destination = rand.Next(1,numNodes + 1);
                distanceMatrix.UpdateAfterEdgeAdded(source, destination, 1);
            }

            Console.WriteLine("DONE!!!!!!");
            Console.ReadKey();
        }

    }
}
