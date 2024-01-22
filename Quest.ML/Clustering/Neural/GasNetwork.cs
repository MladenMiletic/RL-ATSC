using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNetwork
    {
		private int id;
        private List<GasNeuron> neurons;
		private HashSet<Edge> edges;

        public int ID
		{
			get
			{
				return id;
			}
			private set
			{
				id = value;
			}
		}
		public List<GasNeuron> Neurons
		{
			get
			{
				return neurons;
			}
			private set
			{
				neurons = value;
			}
		}
		public HashSet<Edge> Edges
		{
			get
			{
				return edges;
			}
			private set
			{
				edges = value;
			}
        }

		//TODO id should be asigned by the network
		public void AddEdge(int id, GasNeuron source, GasNeuron destination)
		{
			Edge newEdge = new Edge(id, source, destination);


			if (edges.TryGetValue(newEdge, out newEdge)) //TODO test this
			{
				newEdge.ResetAge();
			}
			else
			{
				edges.Add(newEdge);
				//TODO call matrix update
			}
		}
	}
}
