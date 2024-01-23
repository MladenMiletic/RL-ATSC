using Quest.ML.Clustering.Neural;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DistanceMatrix distanceMatrix = new DistanceMatrix();

            distanceMatrix.AddRow(1);
            distanceMatrix.AddRow(2);
            distanceMatrix.AddRow(3,2);
            distanceMatrix.AddRow(4,3);
            distanceMatrix.AddRow(5,4);

            Console.WriteLine(distanceMatrix);

            distanceMatrix.UpdateAfterEdgeAdded(1, 4, 1);

            Console.WriteLine(distanceMatrix);

            //Console.WriteLine(x);
            Console.ReadKey();
        }

    }
}
