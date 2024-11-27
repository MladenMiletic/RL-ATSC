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

                SetUpAgentsWithSingleGNG(9);
                VissimEnvironment env = new VissimEnvironment();
                Random rand = new Random();
                //env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3.inpx\"");
                env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3scenario3.inpx\"");

                Console.WriteLine("Load ok!");
                env.UseMaxSpeed();
                Console.WriteLine("Max speed on!");

                int numberOfRouteDecisions = 36;
                int simRes = env.GetSimulationResolution();
                int initTime = 600;
                int simDuration = env.GetSimulationDuration();
                int controlTimeStep = 300 * simRes;
                int numEpochs = 300;
                double cooperationCoefficient = 1;

                for (int j = 0; j < numEpochs; j++)
                {
                    Console.Write($"Started simulation: {j} ");
                    // DateTime dateTimeBefore = DateTime.Now;
                    env.InitializeSimulation(initTime);
                    int breakTime = initTime;
                    double[] allRequests = env.GetAverageNodeQueue();
                    //State sharing here
                    //allRequests = ProvidePartialStateSharing(allRequests);
                    //var splitRequests = allRequests.Split(4);
                    
                    int stateID = agents[0].GetStateId(allRequests);
                    for (int i = 0; i < agents.Count(); i++)
                    {
                        stateIds[i] = stateID;
                        actionIds[i] = agents[i].SelectAction(stateIds[i]);
                        //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                        //PERFORM ACTION 
                        env.PerformAction(i, actionIds[i]);
                        //GET DELAY BEFORE ACTION
                    }
                    /*
                    for (int i = 0; i < agents.Count(); i++)
                    {
                        stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                        actionIds[i] = agents[i].SelectAction(stateIds[i]);
                        //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                        //PERFORM ACTION 
                        env.PerformAction(i, actionIds[i]);
                        //GET DELAY BEFORE ACTION
                    }
                    */
                    pressuresBeforeActions = env.GetDelaysFromNodes();
                    //pressuresBeforeActions = env.GetVehsFromNodes();
                    while (breakTime < simDuration - controlTimeStep / simRes - 1)
                    {
                        breakTime += controlTimeStep / simRes;
                        env.RunSimulationContinuousWithBreak(breakTime);

                        SaveStateIdsToPreviousStateIds();
                        //GET DELAY AFTER ACTION > CALCULATE REWARD 

                        allRequests = env.GetAverageNodeQueue();
                        //splitRequests = allRequests.Split(4);
                        pressuresAfterActions = env.GetDelaysFromNodes();
                        //pressuresAfterActions = env.GetVehsFromNodes();
                        stateID = agents[0].GetStateId(allRequests);
                        for (int i = 0; i < agents.Count(); i++)
                        {
                            stateIds[i] = stateID;
                            //stateIds[i] = agents[i].GetStateId(splitRequests.ToList()[i].ToArray());
                            //pressuresAfterActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                            //new try with modified reward only afterActionPressureIsUsed
                            rewards[i] = pressuresBeforeActions[i] - pressuresAfterActions[i];
                            //rewards[i] = pressuresAfterActions[i];
                            
                        }
                        //switch here for coop :)
                        rewards = CooperativeReward(rewards, cooperationCoefficient);
                        //CHOSE NEW ACTIONS
                        for (int i = 0; i < agents.Count(); i++)
                        {
                            agents[i].Learn(previousStateIds[i], actionIds[i], stateIds[i], rewards[i]);
                            actionIds[i] = agents[i].SelectAction(stateIds[i]);
                            //Console.WriteLine($"Agent {i}: STATE {stateIds[i]} ACTION: {actionIds[i]}");
                            //PERFORM ACTION
                            env.PerformAction(i, actionIds[i]);
                            //RECORD BEFORE
                            //pressuresBeforeActions[i] = CalculatePressure(splitRequests.ToList()[i].ToArray());
                        }
                        pressuresBeforeActions = env.GetDelaysFromNodes();
                        //pressuresBeforeActions = env.GetVehsFromNodes();
                    }

                    env.RunSimulationContinuous();

                    //DO END OF EPOCH STUFF TO GNG and Q HERE
                    double[] GNGErrors = new double[1];
                    GNGErrors[0] = agents[0].RunGNGTrainingEpoch();
                    int agentId = 0;
                    foreach (var agent in agents)
                    {
                        
                        agentId++;
                        
                        
                        ((ImprovedEpsilonGreedySelectionPolicy)agent.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, j + 1) + 0.05;
                        
                        
                    }
                    agents[0].gasNetworkStateIdentifier.IncrementAgeEdges();
                    ((GaussianNeighbourhoodFunction)agents[0].gasNetworkStateIdentifier.neighbourhoodFunction).NeighbourhoodWidth *= 0.95;
                    agents[0].gasNetworkStateIdentifier.LearningRate = 0.95 * Math.Pow(0.9, j + 1) + 0.05;
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

        private static double[] ProvidePartialStateSharing(double[] allRequests)
        {
            double [] newRequests = new double[allRequests.Length];
            //I1
            newRequests[0] = allRequests[0] + allRequests[4]; //LIJEVO
            newRequests[1] = allRequests[1]; //DESNO
            newRequests[2] = allRequests[2]; //DOLJE
            newRequests[3] = allRequests[3] + allRequests[15]; //GORE
            //I2
            newRequests[4] = allRequests[4] + allRequests[8];
            newRequests[5] = allRequests[5] + allRequests[1];
            newRequests[6] = allRequests[6];
            newRequests[7] = allRequests[7] + allRequests[19];
            //I3
            newRequests[8] = allRequests[8];
            newRequests[9] = allRequests[9] + allRequests[5];
            newRequests[10] = allRequests[10];
            newRequests[11] = allRequests[11] + allRequests[23];
            //I4
            newRequests[12] = allRequests[12] + allRequests[16];
            newRequests[13] = allRequests[13];
            newRequests[14] = allRequests[14] + allRequests[2];
            newRequests[15] = allRequests[15] + allRequests[27];
            //I5
            newRequests[16] = allRequests[16] + allRequests[20];
            newRequests[17] = allRequests[17] + allRequests[13];
            newRequests[18] = allRequests[18] + allRequests[6];
            newRequests[19] = allRequests[19] + allRequests[31];
            //I6
            newRequests[20] = allRequests[20];
            newRequests[21] = allRequests[21] + allRequests[17];
            newRequests[22] = allRequests[22] + allRequests[10];
            newRequests[23] = allRequests[23] + allRequests[35];
            //I7
            newRequests[24] = allRequests[24] + allRequests[28];
            newRequests[25] = allRequests[25];
            newRequests[26] = allRequests[26] + allRequests[14];
            newRequests[27] = allRequests[27];
            //I8
            newRequests[28] = allRequests[28] + allRequests[32];
            newRequests[29] = allRequests[29] + allRequests[25];
            newRequests[30] = allRequests[30] + allRequests[18];
            newRequests[31] = allRequests[31];
            //I9
            newRequests[32] = allRequests[32];
            newRequests[33] = allRequests[33] + allRequests[29];
            newRequests[34] = allRequests[34] + allRequests[22];
            newRequests[35] = allRequests[35];

            return newRequests;
            
        }

        private static double[] CooperativeReward(double[] rewards, double cooperationCoefficient)
        {
            double[] coopRewards = new double[rewards.Length];
            //dont scale, try global reward with coeff 1

            coopRewards[0] = rewards[0] + cooperationCoefficient * (rewards[1] + rewards[3]) / 2.0;
            coopRewards[1] = rewards[1] + cooperationCoefficient * (rewards[0] + rewards[2] + rewards[4]) / 3.0;
            coopRewards[2] = rewards[2] + cooperationCoefficient * (rewards[1] + rewards[5]) / 2.0;
            coopRewards[3] = rewards[3] + cooperationCoefficient * (rewards[0] + rewards[4] + rewards[6]) / 3.0;
            coopRewards[4] = rewards[4] + cooperationCoefficient * (rewards[1] + rewards[3] + rewards[5] + rewards[7]) / 4.0;
            coopRewards[5] = rewards[5] + cooperationCoefficient * (rewards[2] + rewards[4] + rewards[8]) / 3.0;
            coopRewards[6] = rewards[6] + cooperationCoefficient * (rewards[3] + rewards[7]) / 2.0;
            coopRewards[7] = rewards[7] + cooperationCoefficient * (rewards[6] + rewards[4] + rewards[8]) / 3.0;
            coopRewards[8] = rewards[8] + cooperationCoefficient * (rewards[5] + rewards[7]) / 2.0;

            return coopRewards;
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
                GasNetwork network = new GasNetwork(1, 4, 10, 10, 20, 60);
                GaussianNeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();
                network.neighbourhoodFunction = neighbourhoodFunction;
                network.NeuronNumberLimit = 150;
                
                    ISelectionPolicy selectionPolicy = new ImprovedEpsilonGreedySelectionPolicy();
                    agents.Add(new GasQAgent(5, network, selectionPolicy, 0.1, 0.8));
                
            }
        }
        private static void SetUpAgentsWithSingleGNG(int numberOfAgents)
        {
            agents = new List<GasQAgent>();
            stateIds = new int[numberOfAgents];
            previousStateIds = new int[numberOfAgents];
            actionIds = new int[numberOfAgents];
            pressuresAfterActions = new double[numberOfAgents];
            pressuresBeforeActions = new double[numberOfAgents];
            rewards = new double[numberOfAgents];
            GasNetwork network = new GasNetwork(1, 36, 10, 10, 20, 60);
            GaussianNeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();
            network.neighbourhoodFunction = neighbourhoodFunction;
            network.NeuronNumberLimit = 150;
            for (int i = 0; i < numberOfAgents; i++)
            {
                ISelectionPolicy selectionPolicy = new ImprovedEpsilonGreedySelectionPolicy();
                agents.Add(new GasQAgent(5, network, selectionPolicy, 0.1, 0.8));
            }
            network.InitializeNetwork();
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
