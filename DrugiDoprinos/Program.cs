using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.IGNGAgents;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using VissimEnv;
namespace DrugiDoprinos
{
    internal class Program
    {
        private static IGNGQAgent? Agent;
        private static VissimEnvironment? Environment;
        private static List<double[]> StateList = [];
        private static readonly Random rng = new(1337); // To honor NM
        private static readonly int numberOfSimulations = 300;
        private static readonly int initializationTime = 900;
        private static readonly int simulationDuration = 59400;
        private static readonly int agentTimeStep = 300;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            IGNGNetwork gasNetwork = new(8,300);
            ISelectionPolicy selectionPolicy = new EpsilonGreedySelectionPolicy();
            IGNGQAgentParameters gasQAgentParameters = new(4, gasNetwork, selectionPolicy);
            Agent = new IGNGQAgent(gasQAgentParameters);

            Environment = new VissimEnvironment();
            string modelPath = "C:\\Users\\mmiletic\\Desktop\\Simulacijski modeli VISSIM\\model3\\model3_realistic_CV.inpx";
            Environment.LoadNetwork(modelPath);
            Environment.UseMaxSpeed();
            //LOOP iterations

            for (int i = 0; i < numberOfSimulations; i++)
            {
                PerformLearningSimulation();
                SaveResults();
                UpdateParams();
            }
        }

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
                Environment.GetCVDataUMQLS();
                if (j % agentTimeStep != 0)
                {
                    continue;
                }
                
                Console.WriteLine($"NorthAvg: {Environment.NorthQueueLengthAvg}");
                Console.WriteLine($"NorthMax: {Environment.NorthQueueLengthMax}");
                Console.WriteLine($"SouthAvg: {Environment.SouthQueueLengthAvg}");
                Console.WriteLine($"SouthMax: {Environment.SouthQueueLengthMax}");
                Console.WriteLine($"EastAvg: {Environment.EastQueueLengthAvg}");
                Console.WriteLine($"EastMax: {Environment.EastQueueLengthMax}");
                Console.WriteLine($"WestAvg: {Environment.WestQueueLengthAvg}");
                Console.WriteLine($"WestMax: {Environment.WestQueueLengthMax}");
                Environment.ResetTimeStepCounter();
                //StateList.Add(Environment.GetLaneStateInfo());
                //int stateId = Agent.GetStateId(StateList.Last());
                //int actionId = Agent.SelectAction(stateId);
                //Environment.PerformAction(0, actionId);
                if (!firstAction)
                {
                    //double delayAfterAction = Environment.GetAverageDelayOfLastTimeStep();
                    //double reward = delayBeforeAction - delayAfterAction;
                    //delayBeforeAction = delayAfterAction;
                    //Agent.Learn(previousStateId, actionId, stateId, reward);
                    //previousStateId = stateId;
                }
                else
                {
                    //delayBeforeAction = Environment.GetAverageDelayOfLastTimeStep();
                    //previousStateId = stateId;
                    firstAction = false;
                }
            }
        }

        private static void UpdateParams()
        {
            throw new NotImplementedException();
        }

        private static void SaveResults()
        {
            throw new NotImplementedException();
        }


    }
}
