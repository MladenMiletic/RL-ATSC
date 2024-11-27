using Quest.ML.Clustering.Neural.SOM;
using System.Runtime.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrviDoprinos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string filePath = "dataMean.csv"; // Specify the path to your CSV file
            List<double[]> dataList = ReadCSV(filePath);

            Kmeans(dataList);
            //OnlineKmeans(dataList);
            //SOM(dataList);
            //GNG(dataList);
            //IGNG(dataList);
            Console.WriteLine("DONE");
            Console.ReadKey();

        }

        private static void IGNG(List<double[]> dataList)
        {
            throw new NotImplementedException();
        }

        private static void GNG(List<double[]> dataList)
        {
            throw new NotImplementedException();
        }

        private static void SOM(List<double[]> dataList)
        {
            for (int k = 1; k <= 100; k++)
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
                    }
                }
                double rmse = SOM.CalculateRMSE(dataList);
                string rmseFilePath = "SOMrmse.csv";
                SOM.AppendRMSE(rmseFilePath, rmse);


            }
            

        }

        private static void OnlineKmeans(List<double[]> dataList)
        {
            for (int k = 1; k <= 100; k++)
            {


                var (labels, centroids) = OnlineKMeans.PerformOnlineKMeans(dataList, k);
                
                double rmse = KMeans.CalculateRMSE(dataList, labels, centroids);
                string labelsFilePath = "cluster_labels.csv";
                string centroidsFilePath = "centroids.csv";
                string rmseFilePath = "onlineKMeansrmse.csv";

                //KMeans.SaveLabels(labelsFilePath, labels);
                //KMeans.SaveCentroids(centroidsFilePath, centroids);
                OnlineKMeans.AppendRMSE(rmseFilePath, rmse);
            }
        }

        private static void Kmeans(List<double[]> dataList)
        {
            for (int k = 1; k <= 100; k++)
            {

                
                var (labels, centroids) = KMeans.PerformKMeans(dataList, k);
                /*
                // Output results
                Console.WriteLine("Cluster assignments:");
                for (int i = 0; i < labels.Length; i++)
                {
                    Console.WriteLine($"Point {i}: Cluster {labels[i]}");
                }

                Console.WriteLine("\nCentroids:");
                for (int i = 0; i < centroids.Length; i++)
                {
                    Console.WriteLine($"Centroid {i}: [{string.Join(", ", centroids[i])}]");
                }
                */
                double rmse = KMeans.CalculateRMSE(dataList, labels, centroids);
                string labelsFilePath = "cluster_labels.csv";
                string centroidsFilePath = "centroids.csv";
                string rmseFilePath = "kmeansrmse.csv";

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
