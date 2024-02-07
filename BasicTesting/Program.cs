using Quest.ML.Clustering.Neural;
using System.Diagnostics;

namespace BasicTesting
{
    internal class Program
    {
        static void Main()
        {
            Node node1 = new Node();
            Node node2 = new Node();

            Graph<Node, GraphEdge<Node>> graph = [];

            graph.Add(node1);
            graph.Add(node2);

            GraphEdge<Node> edge = new GraphEdge<Node>(node1, node2);
            graph.Edges.Add(edge);

        }

    }
}
