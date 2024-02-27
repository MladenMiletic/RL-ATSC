using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using VissimEnv;

namespace ATSC
{

    public class Program
    {
        public static GasQAgent Agent;
        
        public static void Main(string[] args)
        {
            GasNetworkParameters gasNetworkParameters = new GasNetworkParameters();
            GasNetwork gasNetwork = new GasNetwork(gasNetworkParameters);
            ISelectionPolicy selectionPolicy = new EpsilonGreedySelectionPolicy();
            GasQAgentParameters gasQAgentParameters = new GasQAgentParameters(4, gasNetwork, selectionPolicy);

            Agent = new GasQAgent(gasQAgentParameters);

            VissimEnvironment environment = new VissimEnvironment();




        }
    }
}
