using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using Quest.ML.Extensions;
using VissimEnv;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace MOBDOK
{
    internal class Program
    {
        static int folder = 1;
        public static List<GasQAgent> agents = new List<GasQAgent>();
        public static int[] stateIds;
        public static int[] previousStateIds;
        public static int[] actionIds;
        public static double[] pressuresBeforeActions;
        public static double[] pressuresAfterActions;
        public static double[] rewards;
        static void Main(string[] args)
        {
            SetUpAgents(9);
            VissimEnvironment env = new VissimEnvironment();
            Random rand = new Random();
            env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3.inpx\"");

            Console.WriteLine("Load ok!");
            env.UseMaxSpeed();
            Console.WriteLine("Max speed on!");

            int numberOfRouteDecisions = 36;

            int initTime = 900;
            int simRes = env.GetSimulationResolution();
            int simDuration = env.GetSimulationDuration();
            int controlTimeStep = 60*simRes;
            int numEpochs = 100;
            
            
            for (int j = 0; j < numEpochs; j++)
            {
                Console.Write($"Started simulation: {j} ");
               // DateTime dateTimeBefore = DateTime.Now;
                env.InitializeSimulation(initTime);
                int breakTime = initTime;
                double[] allRequests = env.GetVehicleRouteRequests(numberOfRouteDecisions);
                var splitRequests = allRequests.Split(4);
                for (int i = 0; i < splitRequests.Count(); i++)
                {
                    stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                    actionIds[i] = agents[i].SelectAction(stateIds[i]);
                    //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                    //PERFORM ACTION MISSING
                    env.PerformAction(i, actionIds[i]);
                    //GET DELAY BEFORE ACTION
                    pressuresBeforeActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                }
                while (breakTime < simDuration - controlTimeStep / simRes - 1)
                {
                    breakTime += controlTimeStep / simRes;
                    env.RunSimulationContinuousWithBreak(breakTime);

                    SaveStateIdsToPreviousStateIds();
                    //GET DELAY AFTER ACTION > CALCULATE REWARD 

                    allRequests = env.GetVehicleRouteRequests(numberOfRouteDecisions);
                    splitRequests = allRequests.Split(4);
                    for (int i = 0; i < splitRequests.Count(); i++)
                    {
                        stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                        pressuresAfterActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                        rewards[i] = pressuresBeforeActions[i] - pressuresAfterActions[i];
                        agents[i].Learn(previousStateIds[i], actionIds[i], stateIds[i], rewards[i]);
                    }
                    



                    //CHOSE NEW ACTIONS
                    for (int i = 0; i < splitRequests.Count(); i++)
                    {
                        actionIds[i] = agents[i].SelectAction(stateIds[i]);
                        //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                        //PERFORM ACTION
                        env.PerformAction(i, actionIds[i]);
                        //RECORD BEFORE
                        pressuresBeforeActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                    }

                }
                
                env.RunSimulationContinuous();

                //DO END OF EPOCH STUFF TO GNG and Q HERE
                foreach (var agent in agents)
                {
                    agent.gasNetworkStateIdentifier.IncrementAgeEdges();
                    ((GaussianNeighbourhoodFunction)agent.gasNetworkStateIdentifier.neighbourhoodFunction).NeighbourhoodWidth *= 0.95;
                    agent.gasNetworkStateIdentifier.LearningRate *= 0.95;
                    ((EpsilonGreedySelectionPolicy)agent.SelectionPolicy).Epsilon *= 0.95;
                }
                Console.WriteLine("TTT: " + env.GetTotalTravelTime());
                SaveTTT(env.GetTotalTravelTime(), "TTT.csv");
                
            }



            Console.ReadKey();

        }
        private static double CalculatePressure(double[] requests)
        {
            return requests.Sum();
        }

        private static void SaveStateIdsToPreviousStateIds()
        {
            for (int i = 0;i < stateIds.Length;i++)
            {
                previousStateIds[i] = stateIds[i];
            }
        }

        private static void SetUpAgents(int numberOfAgents)
        {
            agents = new List<GasQAgent>();
            stateIds = new int[numberOfAgents];
            previousStateIds = new int[numberOfAgents];
            actionIds = new int[numberOfAgents];
            pressuresAfterActions = new double[numberOfAgents];
            pressuresBeforeActions = new double[numberOfAgents];
            rewards = new double[numberOfAgents];
            for (int i = 0; i < numberOfAgents; i++)
            {
                GasNetwork network = new GasNetwork(1, 4, 20, 8, 16, 10);
                GaussianNeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();
                network.neighbourhoodFunction = neighbourhoodFunction;
                network.NeuronNumberLimit = 200;
                ISelectionPolicy selectionPolicy = new EpsilonGreedySelectionPolicy();
                agents.Add(new GasQAgent(5, network, selectionPolicy, 0.1, 0.8));
            }
        }

        static void SaveTTT(double TTT, string filePath)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine(TTT);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
        }


        private static void SaveEdges(GasNetwork network, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // Convert double[] to comma-separated string
                    foreach (GasEdge edge in network.Edges)
                    {
                        string line = edge.Source.ID + "," + edge.Target.ID;
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
        }

        private static void SaveWeights(GasNetwork network, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // Convert double[] to comma-separated string
                    foreach (GasNeuron neuron in network)
                    {
                        sw.Write(neuron.ID + ",");
                        string line = string.Join(",", neuron.Weights);
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
