using Quest.ML.Clustering.Neural;
using System.Diagnostics;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DistanceMatrix distanceMatrix = new DistanceMatrix();

            //fill with nodes
            int nodeCount = 100;
            for (int i = 1; i <= nodeCount; i++)
            {
                distanceMatrix.AddRow(i);
            }

            //fill with random edges

            distanceMatrix.ResetReached();
            Random rand = new Random();
            for (int j = 1; j <= 100; j++)
            {
                int a = rand.Next(1, nodeCount + 1);
                int b = rand.Next(1, nodeCount + 1);
                distanceMatrix.UpdateAfterEdgeAdded(a,b,1);
            }
            Console.WriteLine("DONE");
            //Console.WriteLine(distanceMatrix);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int x = rand.Next(1, nodeCount + 1);
            int y = rand.Next(1, nodeCount + 1);
            distanceMatrix.UpdateAfterEdgeAdded(x, y, 1);
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }

    }
}
