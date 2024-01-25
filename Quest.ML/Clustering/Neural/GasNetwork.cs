using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class GasNetwork
    {
		private int id;
        private List<GasNeuron> neurons;
		private HashSet<Edge> edges;
		private DistanceMatrix distanceMatrix;

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
		public DistanceMatrix DistanceMatrix
		{
			get
			{
				return distanceMatrix;
			}
			private set
			{
				distanceMatrix = value;
			}
		}

        //TODO id should be asigned by the network
        private void AddNeuronWithEdge(int id, GasNeuron source, GasNeuron destination)
		{
			neurons.Add(source);
			AddEdge(id,source,destination);
            DistanceMatrix.AddRow(source.ID, destination.ID);
        }

		private void AddOrUpdateEdge(int id, GasNeuron source, GasNeuron destination)
		{
			AddEdge(id,source,destination);
			//TODO UPDATE DISTANCE MATRIX
			DistanceMatrix.UpdateAfterEdgeAdded(source.ID, destination.ID);
		}

		private void AddEdge(int id, GasNeuron source, GasNeuron destination)
		{
			Edge newEdge = new Edge(id,source,destination);
            if (edges.TryGetValue(newEdge, out newEdge)) //TODO test this
            {
                newEdge.ResetAge();
            }
            else
            {
                edges.Add(newEdge);
            }
        }

		private void RemoveEdge(Edge edge)
		{
			edges.Remove(edge);
			GasNeuron source = edge.Source;
			GasNeuron destination = edge.Destination;

			if (!DistanceMatrix.IsConnected(source.ID))
			{
				RemoveNeuron(source);
				DistanceMatrix.RemoveRow(source.ID);
			}
			if (!DistanceMatrix.IsConnected(destination.ID))
			{
				RemoveNeuron(destination);
				DistanceMatrix.RemoveRow(destination.ID);
			}
			//TODO UPDATE MATRIX AFTER DELETION

		}

        private void AddNeuronWithoutEdge(GasNeuron source)
		{
			neurons.Add(source);
			DistanceMatrix.AddRow(source.ID);

		}

		private void RemoveNeuron(GasNeuron neuron)
		{
			neurons.Remove(neuron);
		}
		
	}
}
