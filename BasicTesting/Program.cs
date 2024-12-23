﻿using Quest.ML.Clustering.Neural;
using Quest.ML.Clustering.Neural.NeighbourhoodFunctions;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BasicTesting
{
    internal class Program
    {
        static int folder = 1;
        public static void Main()
        {
            //Inputs with 10000 random points //2-Dimensional
            //Run several GNG epochs, track avg error
            //Export error per epoch
            //Export neuron weights
            string filePath = "synDataForTest.csv"; // Specify the path to your CSV file
            List<double[]> dataList = ReadCSV(filePath);
            GasNetwork network = new GasNetwork(1, 2, 50, 1, 3, 2);
            network.NeuronAdded += HandleNeuronAddition;
            network.NeuronDeleted += HandleNeuronDeletion;
            GaussianNeighbourhoodFunction neighbourhoodFunction = new GaussianNeighbourhoodFunction();
            network.neighbourhoodFunction = neighbourhoodFunction;
            network.InitializeNetwork();
            network.NeuronNumberLimit = 9;
            double[] errors = new double[1000];
            for (int i = 0; i < 100; i++)
            {
                SaveWeights(network, $"weights{folder}.csv");
                SaveEdges(network, $"edges{folder}.csv");
                folder++;
                errors[i] = RunEpoch(dataList, network);
                network.IncrementAgeEdges();
                neighbourhoodFunction.NeighbourhoodWidth *= 0.7;
                network.LearningRate *= 0.7;

                SaveError(errors, $"error{i}.csv");
                
                
            }
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