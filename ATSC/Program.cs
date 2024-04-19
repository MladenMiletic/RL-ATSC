using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using VissimEnv;

namespace ATSC
{

    public class Program
    {
        private static GasQAgent? Agent;
        private static VissimEnvironment? Environment;
        private static List<double[]> StateList = [];
        private static readonly Random rng = new();

        private static readonly int numberOfSimulations = 500;
        private static readonly int initializationTime = 900;
        private static readonly int simulationDuration = 59400;
        private static readonly int agentTimeStep = 300;
        
        public static void Main()
        {
            GasNetworkParameters gasNetworkParameters = new();
            GasNetwork gasNetwork = new(gasNetworkParameters);
            ISelectionPolicy selectionPolicy = new EpsilonGreedySelectionPolicy();
            GasQAgentParameters gasQAgentParameters = new(4, gasNetwork, selectionPolicy);

            Agent = new GasQAgent(gasQAgentParameters);
            Environment = new VissimEnvironment();

            for (int i = 0; i <= numberOfSimulations; i++)
            {
                PerformLearningSimulation();
                var RandomStateListPermutation = StateList.OrderBy(_ => rng.Next()).ToList();
                var gasNetworkError = RunGasNetworkLearningEpoch(RandomStateListPermutation, Agent.gasNetworkStateIdentifier);
                //DO SOMETHING WITH NETWORK PARAMETERS (Neighbourhood function, learning rate)

                //DO SOMETHING WITH RESULTS
            }
        }
        /// <summary>
        /// this will perform one learning simulation
        /// </summary>
        /// <exception cref="NullReferenceException">Agent and the environment must not be null</exception>
        private static void PerformLearningSimulation()
        {
            
            if (Agent == null)
            {
                throw new NullReferenceException("Agent can not be null during learning simulation!");
            }
            if (Environment == null)
            {
                throw new NullReferenceException("Environment can not be null during learning simulation!");
            }

            StateList = [];
            bool firstAction = true;
            double delayBeforeAction = 0;
            int previousStateId = 0;
            Environment.InitializeSimulation(initializationTime);
            for (int j = 0; j < simulationDuration - initializationTime; j++)
            {
                Environment.RunSimulationStep();
                if (j == simulationDuration - initializationTime - 1)
                {
                    break;
                }
                if (j % agentTimeStep != 0)
                {
                    continue;
                }
                StateList.Add(Environment.GetLaneStateInfo());
                int stateId = Agent.GetStateId(StateList.Last());
                int actionId = Agent.SelectAction(stateId);
                Environment.PerformAction(actionId);
                if (!firstAction)
                {
                    double delayAfterAction = Environment.GetAverageDelayOfLastTimeStep();
                    double reward = delayBeforeAction - delayAfterAction;
                    delayBeforeAction = delayAfterAction;
                    Agent.Learn(previousStateId, actionId, stateId, reward);
                    previousStateId = stateId;
                }
                else
                {
                    delayBeforeAction = Environment.GetAverageDelayOfLastTimeStep();
                    previousStateId = stateId;
                    firstAction = false;
                }
            }
        }
        private static double RunGasNetworkLearningEpoch(List<double[]> dataList, GasNetwork network)
        {
            double error = 0;
            foreach (double[] data in dataList)
            {
                error += network.Learn(data);
            }
            network.IncrementAgeEdges();
            return error / dataList.Count;
        }
    }
}
