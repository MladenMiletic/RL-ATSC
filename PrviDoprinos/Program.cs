using Quest.ML.Clustering.Neural.GNG;
using Quest.ML.Clustering.Neural.SOM;
using Quest.ML.Extensions;
using System.Runtime.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrviDoprinos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string filePath = "dataMax.csv"; // Specify the path to your CSV file
            List<double[]> dataList = ReadCSV(filePath);
            //dataList.Shuffle();
            //Kmeans(dataList);
            Console.WriteLine("Done K-means");
            //OnlineKmeans(dataList);
            Console.WriteLine("Done Online K-means");
            //SOM(dataList);
            Console.WriteLine("Done SOM");
            //GNG(dataList);
            Console.WriteLine("Done GNG");
            IGNG(dataList);
            Console.WriteLine("DONE IGNG");
            Console.ReadKey();

        }

        private static void IGNG(List<double[]> dataList)
        {
            for (int k = 100; k <= 100; k++)
            {

                IGNGNetwork IGNG = new IGNGNetwork(4, k);
                IGNG.maxAge = 7;
                int iterations = 100;
                IGNG.growingDistance = 75;
                IGNG.maturationAge = 5;
                Random r = new Random(1);
                for (int j = 0; j < IGNG.Nodes.Count; j++)
                {
                    IGNG.Nodes[j].SetWeights(dataList[r.Next(1, dataList.Count)]);
                }
                for (int i = 0; i < iterations; i++)
                {

                    foreach (double[] data in dataList)
                    {

                        IGNG.Learn(data);
                        double rmse = IGNG.CalculateRMSE(dataList);

                        string rmseFilePath = "RMSEigng.csv";
                        IGNG.AppendRMSE(rmseFilePath, rmse);

                    }

                }
                List<IGNGNeuron> neuronsForDeletion = new List<IGNGNeuron>();
                foreach (IGNGNeuron neuron in IGNG)
                {
                    if (neuron.age <= IGNG.maturationAge)
                    {
                        neuronsForDeletion.Add(neuron);
                    }
                }
                foreach (IGNGNeuron iGNGNeuron in neuronsForDeletion)
                {
                    IGNG.Remove(iGNGNeuron);
                }
                //double rmse = IGNG.CalculateRMSE(dataList);
                //int[] labels = IGNG.GetLabels(dataList);
                //string labelsFilePath = "LABELSIGNG.csv";
                //KMeans.SaveLabels(labelsFilePath, labels);
                //string rmseFilePath = "RMSEigng.csv";
                //IGNG.AppendRMSE(rmseFilePath, rmse);
                //Console.WriteLine(IGNG.Count);
                //Console.WriteLine(rmse);

            }
        }

        private static void GNG(List<double[]> dataList)
        {
            for (int k = 100; k <= 100; k++)
            {

                GNGNetwork GNG = new GNGNetwork(4, k);
                GNG.errorUpdate = 0.5;
                GNG.errorDecay = 0.99;
                GNG.maxAge = 7;
                int iterations = 100;
                Random r = new Random(1);
                for (int j = 0; j < GNG.Nodes.Count; j++)
                {
                    GNG.Nodes[j].SetWeights(dataList[r.Next(1, dataList.Count)]);
                }
                int z = 0;
                for (int i = 0; i < iterations; i++)
                {

                    foreach (double[] data in dataList)
                    {
                        z++;

                        GNG.Learn(data);

                        if (z % 46 == 0)
                        {
                            GNG.Update();
                        }
                        foreach (GNGNeuron neuron in GNG.Nodes)
                        {
                            neuron.Error = neuron.Error * GNG.errorDecay;
                        }
                        double rmse = GNG.CalculateRMSE(dataList);
                        
                        string rmseFilePath = "RMSEgng.csv";
                        GNG.AppendRMSE(rmseFilePath, rmse);
                    }

                }
                //double rmse = GNG.CalculateRMSE(dataList);
                //int[] labels = GNG.GetLabels(dataList);
                //string labelsFilePath = "LABELSGNG.csv";
                //KMeans.SaveLabels(labelsFilePath, labels);
                //string rmseFilePath = "RMSEgng.csv";
                //GNG.AppendRMSE(rmseFilePath, rmse);

                
            }
        }

        private static void SOM(List<double[]> dataList)
        {
            for (int k = 100; k <= 100; k++)
            {
                SOMNetwork SOM = new SOMNetwork(4, k);
                int iterations = 100;
                Random r = new Random(1);
                for (int j = 0; j < k; j++)
                {
                    
                    SOM.Nodes[j].SetWeights(dataList[r.Next(1,dataList.Count)]);
                }
                for (int i = 0; i < iterations; i++)
                {

                    foreach (double[] data in dataList)
                    {

                        SOM.Learn(data);
                        double rmse = SOM.CalculateRMSE(dataList);
                        string rmseFilePath = "RMSEsom.csv";
                        SOM.AppendRMSE(rmseFilePath, rmse);
                    }
                }
                //double rmse = SOM.CalculateRMSE(dataList);
                //int[] labels = SOM.GetLabels(dataList);
                //string labelsFilePath = "LABELSSOM.csv";
                //KMeans.SaveLabels(labelsFilePath, labels);
                //string rmseFilePath = "RMSEsom.csv";
                //SOM.AppendRMSE(rmseFilePath, rmse);


            }
            

        }

        private static void OnlineKmeans(List<double[]> dataList)
        {
            for (int k = 100; k <= 100; k++)
            {


                var (labels, centroids) = OnlineKMeans.PerformOnlineKMeans(dataList, k);
                
                double rmse = KMeans.CalculateRMSE(dataList, labels, centroids);
                string labelsFilePath = "LABELSonlinekmeans.csv";
                string centroidsFilePath = "centroids.csv";
                string rmseFilePath = "RMSEonlinekmeans.csv";

                //KMeans.SaveLabels(labelsFilePath, labels);
                //KMeans.SaveCentroids(centroidsFilePath, centroids);
                //OnlineKMeans.AppendRMSE(rmseFilePath, rmse);
            }
        }

        private static void Kmeans(List<double[]> dataList)
        {
            for (int k = 100; k <= 100; k++)
            {

                
                var (labels, centroids) = KMeans.PerformKMeans(dataList, k);

                double rmse = KMeans.CalculateRMSE(dataList, labels, centroids);
                string labelsFilePath = "LABELSkmeans.csv";
                string centroidsFilePath = "centroids.csv";
                string rmseFilePath = "RMSEkmeans.csv";

                //KMeans.SaveLabels(labelsFilePath, labels);
                //KMeans.SaveCentroids(centroidsFilePath, centroids);
                KMeans.AppendRMSE(rmseFilePath, rmse);
            } 
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
    }
}
