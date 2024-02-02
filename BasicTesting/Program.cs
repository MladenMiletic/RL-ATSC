using Quest.ML.Clustering.Neural;
using System.Diagnostics;
using PlotGraph;
using MathWorks.MATLAB.NET.Arrays;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
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
            

            CaricTree<int> stablo = new CaricTree<int>(1, 1);

            Console.WriteLine($"Trenutno stablo ima {stablo.Count} elemenata");

            CaricNode<int> noviNode = new CaricNode<int>(2, 2);

            stablo.AddNode(stablo.Root, noviNode);
            stablo.AddNode(stablo.Root, new CaricNode<int>(3, 3));
            stablo.AddNode(stablo.Root, new CaricNode<int>(4, 4));

            stablo.AddNode(stablo.Root.Children[3], new CaricNode<int>(5, 5));

            Console.WriteLine($"Trenutno stablo ima {stablo.Count} elemenata");

            Console.WriteLine(stablo);
            */

            //Ovo su neurončeki za testiranje
            GasNeuron n1 = new GasNeuron(1, 8);
            GasNeuron n2 = new GasNeuron(2, 8);
            GasNeuron n3 = new GasNeuron(3, 8);
            GasNeuron n4 = new GasNeuron(4, 8);
            GasNeuron n5 = new GasNeuron(5, 8);
            GasNeuron n6 = new GasNeuron(6, 8);
            GasNeuron n7 = new GasNeuron(7, 8);
            

            CaricTree<GasNeuron> stabloN1 = new CaricTree<GasNeuron>(n1.ID, n1);
            CaricTree<GasNeuron> stabloN2 = new CaricTree<GasNeuron>(n2.ID, n2);
            CaricTree<GasNeuron> stabloN3 = new CaricTree<GasNeuron>(n3.ID, n3);
            CaricTree<GasNeuron> stabloN4 = new CaricTree<GasNeuron>(n4.ID, n4);
            CaricTree<GasNeuron> stabloN5 = new CaricTree<GasNeuron>(n5.ID, n5);
            CaricTree<GasNeuron> stabloN6 = new CaricTree<GasNeuron>(n6.ID, n6);
            CaricTree<GasNeuron> stabloN7 = new CaricTree<GasNeuron>(n7.ID, n7);

            stabloN1.AddNode(1, new CaricNode<GasNeuron>(n2.ID, n2));
            stabloN1.AddNode(2, new CaricNode<GasNeuron>(n3.ID, n3));
            stabloN1.AddNode(3, new CaricNode<GasNeuron>(n4.ID, n4));
            stabloN1.AddNode(3, new CaricNode<GasNeuron>(n5.ID, n5));
            stabloN1.AddNode(5, new CaricNode<GasNeuron>(n6.ID, n6));

            //stabloN2.AddNode(stabloN2.Root, new CaricNode<GasNeuron>(n1.ID, n1));
            



            Console.WriteLine(stabloN1);
            //Console.WriteLine(stabloN2);
            MatlabGrapher matlab = new MatlabGrapher();
            double[] source = {1,2,3,3,5};
            double[] destination = {2,3,4,5,6};
            MWNumericArray AIn = new MWNumericArray(source);
            MWNumericArray BIn = new MWNumericArray(destination);
            matlab.PlotGraph(AIn, BIn);


            Console.ReadKey();
        }

    }
}
