using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using Quest.ML.Extensions;
using VissimEnv;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Net;

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
            try
            {

                SetUpAgents(9);
                VissimEnvironment env = new VissimEnvironment();
                Random rand = new Random();
                env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3.inpx\"");

                Console.WriteLine("Load ok!");
                env.UseMaxSpeed();
                Console.WriteLine("Max speed on!");

                int numberOfRouteDecisions = 36;
                int simRes = env.GetSimulationResolution();
                int initTime = 600;
                int simDuration = env.GetSimulationDuration();
                int controlTimeStep = 300 * simRes;
                int numEpochs = 500;


                for (int j = 0; j < numEpochs; j++)
                {
                    Console.Write($"Started simulation: {j} ");
                    // DateTime dateTimeBefore = DateTime.Now;
                    env.InitializeSimulation(initTime);
                    int breakTime = initTime;
                    double[] allRequests = env.GetAverageNodeQueue();
                    var splitRequests = allRequests.Split(4);
                    for (int i = 0; i < splitRequests.Count(); i++)
                    {
                        stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                        actionIds[i] = agents[i].SelectAction(stateIds[i]);
                        //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                        //PERFORM ACTION 
                        env.PerformAction(i, actionIds[i]);
                        //GET DELAY BEFORE ACTION
                    }
                    pressuresBeforeActions = env.GetDelaysFromNodes();
                    while (breakTime < simDuration - controlTimeStep / simRes - 1)
                    {
                        breakTime += controlTimeStep / simRes;
                        env.RunSimulationContinuousWithBreak(breakTime);

                        SaveStateIdsToPreviousStateIds();
                        //GET DELAY AFTER ACTION > CALCULATE REWARD 

                        allRequests = env.GetAverageNodeQueue();
                        splitRequests = allRequests.Split(4);
                        pressuresAfterActions = env.GetDelaysFromNodes();
                        for (int i = 0; i < splitRequests.Count(); i++)
                        {
                            stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                            //pressuresAfterActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                            //new try with modified reward only afterActionPressureIsUsed
                            //rewards[i] = pressuresBeforeActions[i] - pressuresAfterActions[i];
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
                            //pressuresBeforeActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                        }
                        pressuresBeforeActions = env.GetDelaysFromNodes();
                    }

                    env.RunSimulationContinuous();

                    //DO END OF EPOCH STUFF TO GNG and Q HERE
                    double[] GNGErrors = new double[9];
                    int agentId = 0;
                    foreach (var agent in agents)
                    {
                        GNGErrors[agentId] = agent.RunGNGTrainingEpoch();
                        agentId++;
                        agent.gasNetworkStateIdentifier.IncrementAgeEdges();
                        ((GaussianNeighbourhoodFunction)agent.gasNetworkStateIdentifier.neighbourhoodFunction).NeighbourhoodWidth *= 0.95;
                        agent.gasNetworkStateIdentifier.LearningRate = 0.95 * Math.Pow(0.9, j + 1) + 0.05;
                        ((ImprovedEpsilonGreedySelectionPolicy)agent.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, j + 1) + 0.05;
                    }
                    Console.WriteLine("TTT: " + env.GetTotalTravelTime());
                    SaveResults(env.GetTotalTravelTime(), "TTT.csv");
                    SaveResults(env.GetAverageDelay(), "Delay.csv");
                    SaveResults(env.GetTotalNumberOfStops(), "Stops.csv");
                    SaveResults(env.GetTotalDelaysFromNodes(), "NodeDelays.csv");
                    SaveResults(GNGErrors, "GNGErrors.csv");
                    
                    for (int i = 0; i < agents.Count; i++)
                    {
                        SaveEdges(agents[i].gasNetworkStateIdentifier, $"{j}Agent{i} Edges.csv");
                        SaveWeights(agents[i].gasNetworkStateIdentifier, $"{j}Agent{i} Weights.csv");
                        SaveQValues(agents[i], $"{j}Agent{i} Qvalues.csv");
                    }
                }
                Console.ReadKey();
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message);
                using (StreamWriter sw = new StreamWriter("errordata.txt"))
                {
                    sw.WriteLine(e.Message + "\n");
                    sw.WriteLine(e.StackTrace + "\n");
                    sw.WriteLine(e.TargetSite + "\n");
                    
                }
            }
        }

        private static void SaveQValues(GasQAgent gasQAgent, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (var line in gasQAgent.QTable)
                    {
                        foreach (var value in line.Value)
                        {
                            sw.Write(value + ",");
                        }
                        sw.WriteLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
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
                GasNetwork network = new GasNetwork(1, 4, 20, 20, 40, 30);
                GaussianNeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();
                network.neighbourhoodFunction = neighbourhoodFunction;
                network.NeuronNumberLimit = 65;
                ISelectionPolicy selectionPolicy = new ImprovedEpsilonGreedySelectionPolicy();
                agents.Add(new GasQAgent(5, network, selectionPolicy, 0.1, 0.8));
            }
        }

        static void SaveResults(double TTT, string filePath)
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
        static void SaveResults(double[] doubleArray, string filePath)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filePath, true);
                string line = string.Join(",", doubleArray);
                sw.WriteLine(line);
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
                        sw.Write(line);
                        sw.WriteLine("," + neuron.Mature.ToString());
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
