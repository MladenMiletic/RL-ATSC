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

		private List<GasNeuron> neurons;

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


	}
}
