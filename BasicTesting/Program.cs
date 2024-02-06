using Quest.ML.Clustering.Neural;
using System.Diagnostics;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CaricNetwork<int> net = new CaricNetwork<int>();

            net.AddUnconnetedNode(1);

            Console.WriteLine(net);

            Console.WriteLine();

            net.AddConnectedNode(0, 1, 2);
            net.AddConnectedNode(0, 1, 3);

            Console.WriteLine(net);

            Console.ReadKey();
        }

    }
}
