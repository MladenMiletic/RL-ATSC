using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using System.Diagnostics;
using VissimEnv;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BasicTesting
{
    internal class Program
    {
        static int folder = 1;
        public static void Main()
        {
            VissimEnvironment env = new VissimEnvironment();
            Random rand = new Random();
            env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3.inpx\"");

            int numberOfRouteDecisions = 36;
            int simRes = env.GetSimulationResolution();
            int initTime = 300;
            int simDuration = env.GetSimulationDuration();
            int controlTimeStep = 60 * simRes;
            int numEpochs = 500;
            env.UseMaxSpeed();
            env.InitializeSimulation(initTime);

            double[] q = env.GetDelaysFromNodes();
            Console.ReadKey();
        }

        private static void Network_NeuronAdded(object? sender, GasNeuron e)
        {
            throw new NotImplementedException();
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

        private static double RunEpoch(List<double[]> dataList, GasNetwork network)
        {

            double error = 0;
            int i = 0;
            foreach (double[] data in dataList)
            {
                error += network.Learn(data);
                //SaveWeights(network, $"weights{folder}.csv");
                //SaveEdges(network, $"edges{folder}.csv");
                if (i % 10 == 0)
                {
                    SaveWeights(network, $"weights{folder}.csv");
                    SaveEdges(network, $"edges{folder}.csv");
                    folder++;
                }
                i++;
            }
            return error / dataList.Count;
        }

        static List<double[]> ReadCSV(string filePath)
        {
            List<double[]> dataList = [];

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        double[] doubles = Array.ConvertAll(values, Double.Parse);
                        dataList.Add(doubles);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return dataList;
        }

        static void SaveError(double[] error, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // Convert double[] to comma-separated string
                    foreach (double data in error)
                    {
                        sw.WriteLine(data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while writing the CSV file:");
                Console.WriteLine(e.Message);
            }
        }

        private static void HandleNeuronAddition(object? sender, GasNeuron addedNeuron)
        {
            Console.WriteLine($"ADD: NEURON ID: {addedNeuron.ID}");
        }
        private static void HandleNeuronDeletion(object? sender, GasNeuron deletedNeuron)
        {
            Console.WriteLine($"REMOVE: NEURON ID: {deletedNeuron.ID}");
        }

    }
}
