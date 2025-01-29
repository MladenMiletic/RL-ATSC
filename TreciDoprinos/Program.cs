using Microsoft.VisualBasic;
using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.IGNGAgents;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using System;
using System.Runtime.CompilerServices;
using VissimEnv;

namespace TreciDoprinos
{
    internal class Program
    {
        private static IGNGQAgent? VSLAgent;
        private static IGNGQAgent? ATSCAgent1;
        private static IGNGQAgent? ATSCAgent2;
        private static IGNGQAgent? ATSCAgent3;
        private static VissimEnvironment? Environment;
        private static readonly Random rng = new(1337); // To honor NM
        private static readonly int numberOfSimulations = 500;
        private static readonly int initializationTime = 900;
        private static readonly int simulationDuration = 4500;
        private static readonly int agentTimeStep = 180;

        private static readonly double growingDistanceVSL = 10;
        private static readonly double growingDistanceATSC1 = 10;
        private static readonly double growingDistanceATSC2 = 10;
        private static readonly double growingDistanceATSC3 = 10;
       
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            IGNGNetwork vslNetwork, atscNetwork1, atscNetwork2, atscNetwork3;
            InitializeNetworksAndAgents(out vslNetwork, out atscNetwork1, out atscNetwork2, out atscNetwork3);

            Environment = new VissimEnvironment();

            string modelPath = "C:\\Users\\mmiletic\\Desktop\\Simulacijski modeli VISSIM\\Lucko\\model.inpx";
            Environment.LoadNetwork(modelPath);
            Environment.UseMaxSpeed();

            //LOOP iterations
            for (int i = 0; i < numberOfSimulations; i++)
            {
                PerformLearningSimulation();
                SaveResults();
                UpdateParams(i);
            }
            VSLAgent.SaveQTable("QTableVSL.csv");
            vslNetwork.SaveNetwork("NetworkVSL.csv");
            ATSCAgent1.SaveQTable("QTableATSC1.csv");
            atscNetwork1.SaveNetwork("NetworkATSC1.csv");
            ATSCAgent2.SaveQTable("QTableATSC2.csv");
            atscNetwork2.SaveNetwork("NetworkATSC2.csv");
            ATSCAgent3.SaveQTable("QTableATSC3.csv");
            atscNetwork3.SaveNetwork("NetworkATSC3.csv");




        }
        /*/
        static void Main(string[] args)
        {
            Console.WriteLine("TEST, World!");
            IGNGNetwork vslNetwork, atscNetwork1, atscNetwork2, atscNetwork3;
            InitializeNetworksAndAgents(out vslNetwork, out atscNetwork1, out atscNetwork2, out atscNetwork3);

            Environment = new VissimEnvironment();

            string modelPath = "C:\\Users\\mmiletic\\Desktop\\Simulacijski modeli VISSIM\\Lucko\\model.inpx";
            Environment.LoadNetwork(modelPath);


            Environment.RunSimulationContinuousWithBreak(1200);
            Environment.ApplyVariableSpeedLimit(7);
            Environment.GetDelayVSL();
            double[] state = Environment.GetStateLucko();
            foreach (var item in state)
            {
                Console.WriteLine(item);
            }



        }*/
        private static void UpdateParams(int iteration)
        {
            ((EpsilonGreedySelectionPolicy)VSLAgent.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, iteration + 1) + 0.05;
            VSLAgent.RunGNGTrainingEpoch();
            ((EpsilonGreedySelectionPolicy)ATSCAgent1.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, iteration + 1) + 0.05;
            ATSCAgent1.RunGNGTrainingEpoch();
            ((EpsilonGreedySelectionPolicy)ATSCAgent2.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, iteration + 1) + 0.05;
            ATSCAgent2.RunGNGTrainingEpoch();
            ((EpsilonGreedySelectionPolicy)ATSCAgent3.SelectionPolicy).Epsilon = 0.95 * Math.Pow(0.9, iteration + 1) + 0.05;
            ATSCAgent3.RunGNGTrainingEpoch();
        }

        private static void SaveResults()
        {
            SaveData(Environment.GetTotalTravelTime(), "TTT.csv");
            SaveData(Environment.GetAverageDelay(), "Delay.csv");
            SaveData(Environment.GetTotalNumberOfStops(), "Stops.csv");
            Console.WriteLine(Environment.GetTotalTravelTime());
        }

        static void SaveData(double data, string filePath)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
        }

        private static void PerformLearningSimulation()
        {
            bool firstAction = true;
            double delayBeforeActionVSL = 0;
            double delayBeforeActionATSC1 = 0;
            double delayBeforeActionATSC2 = 0;
            double delayBeforeActionATSC3 = 0;
            int previousStateIdVSL = 0;
            int previousStateIdATSC1 = 0;
            int previousStateIdATSC2 = 0;
            int previousStateIdATSC3 = 0;
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

                double[] rawState = Environment.GetStateLucko();

                int stateIdVSL = VSLAgent.GetStateId(rawState);
                int actionIdVSL = VSLAgent.SelectAction(stateIdVSL);

                int stateIdATSC1 = ATSCAgent1.GetStateId(rawState);
                int actionIdATSC1 = ATSCAgent1.SelectAction(stateIdATSC1);

                int stateIdATSC2 = ATSCAgent2.GetStateId(rawState);
                int actionIdATSC2 = ATSCAgent2.SelectAction(stateIdATSC2);

                int stateIdATSC3 = ATSCAgent3.GetStateId(rawState);
                int actionIdATSC3 = ATSCAgent3.SelectAction(stateIdATSC3);

                Environment.ApplyVariableSpeedLimit(actionIdVSL);
                Environment.SelectSignalProgram(0, actionIdATSC1);
                Environment.SelectSignalProgram(1, actionIdATSC2);
                Environment.SelectSignalProgram(2, actionIdATSC3);
                if (!firstAction)
                {
                    double delayAfterActionVSL = Environment.GetDelayVSL();
                    double delayAfterActionATSC1 = Environment.GetDelayATSC1();
                    double delayAfterActionATSC2 = Environment.GetDelayATSC2();
                    double delayAfterActionATSC3 = Environment.GetDelayATSC3();

                    double rewardVSL = delayBeforeActionVSL - delayAfterActionVSL;
                    double rewardATSC1 = delayBeforeActionATSC1 - delayAfterActionATSC1;
                    double rewardATSC2 = delayBeforeActionATSC2 - delayAfterActionATSC2;
                    double rewardATSC3 = delayBeforeActionATSC3 - delayAfterActionATSC3;

                    delayBeforeActionVSL = delayAfterActionVSL;
                    delayBeforeActionATSC1 = delayAfterActionATSC1;
                    delayBeforeActionATSC2 = delayAfterActionATSC2;
                    delayBeforeActionATSC3 = delayAfterActionATSC3;

                    VSLAgent.Learn(previousStateIdVSL, actionIdVSL, stateIdVSL, rewardVSL);
                    ATSCAgent1.Learn(previousStateIdATSC1, actionIdATSC1, stateIdATSC1, rewardATSC1);
                    ATSCAgent2.Learn(previousStateIdATSC2, actionIdATSC2, stateIdATSC2, rewardATSC2);
                    ATSCAgent3.Learn(previousStateIdATSC3, actionIdATSC3, stateIdATSC3, rewardATSC3);
                }
                else
                {
                    delayBeforeActionVSL = Environment.GetDelayVSL();
                    delayBeforeActionATSC1 = Environment.GetDelayATSC1();
                    delayBeforeActionATSC2 = Environment.GetDelayATSC2();
                    delayBeforeActionATSC3 = Environment.GetDelayATSC3();

                    previousStateIdVSL = stateIdVSL;
                    previousStateIdATSC1 = stateIdATSC1;
                    previousStateIdATSC2 = stateIdATSC2;
                    previousStateIdATSC3 = stateIdATSC3;
                    firstAction = false;
                }
            }
        }

        private static void InitializeNetworksAndAgents(out IGNGNetwork vslNetwork, out IGNGNetwork atscNetwork1, out IGNGNetwork atscNetwork2, out IGNGNetwork atscNetwork3)
        {
            //NETs for all
            vslNetwork = new(3, 50);
            atscNetwork1 = new(15, 100);
            atscNetwork2 = new(7, 100);
            atscNetwork3 = new(7, 100);
            vslNetwork.growingDistance = growingDistanceVSL;
            atscNetwork1.growingDistance = growingDistanceATSC1;
            atscNetwork2.growingDistance = growingDistanceATSC2;
            atscNetwork3.growingDistance = growingDistanceATSC3;


            //Selection policies
            ISelectionPolicy selectionPolicyVSL = new EpsilonGreedySelectionPolicy();
            ISelectionPolicy selectionPolicyATSC1 = new EpsilonGreedySelectionPolicy();
            ISelectionPolicy selectionPolicyATSC2 = new EpsilonGreedySelectionPolicy();
            ISelectionPolicy selectionPolicyATSC3 = new EpsilonGreedySelectionPolicy();

            IGNGQAgentParameters vslQAgentParameters = new(10, vslNetwork, selectionPolicyVSL);
            IGNGQAgentParameters atscQAgentParameters1 = new(5, atscNetwork1, selectionPolicyATSC1);
            IGNGQAgentParameters atscQAgentParameters2 = new(5, atscNetwork2, selectionPolicyATSC2);
            IGNGQAgentParameters atscQAgentParameters3 = new(5, atscNetwork3, selectionPolicyATSC3);

            VSLAgent = new IGNGQAgent(vslQAgentParameters);
            ATSCAgent1 = new IGNGQAgent(atscQAgentParameters1);
            ATSCAgent2 = new IGNGQAgent(atscQAgentParameters2);
            ATSCAgent3 = new IGNGQAgent(atscQAgentParameters3);
        }
    }
}
