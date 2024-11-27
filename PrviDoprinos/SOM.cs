using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class SOM
{
    public static (int[], double[][]) PerformSOM(
        List<double[]> data, int gridRows, int gridCols, double initialLearningRate, double initialRadius, int maxIterations)
    {
        int numPoints = data.Count;
        int dimensions = data[0].Length;

        // Initialize grid of centroids randomly
        Random random = new Random();
        double[][] centroids = new double[gridRows * gridCols][];
        for (int i = 0; i < centroids.Length; i++)
        {
            centroids[i] = new double[dimensions];
            for (int d = 0; d < dimensions; d++)
            {
                centroids[i][d] = random.NextDouble();
            }
        }

        int[] labels = new int[numPoints];

        // Perform SOM
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            double learningRate = initialLearningRate * Math.Exp(-iteration / (double)maxIterations);
            double radius = initialRadius * Math.Exp(-iteration / (double)maxIterations);

            for (int i = 0; i < numPoints; i++)
            {
                // Find the Best Matching Unit (BMU)
                int bmuIndex = FindBMU(data[i], centroids);

                // Update the BMU and its neighbors
                for (int j = 0; j < centroids.Length; j++)
                {
                    double distanceToBMU = GridDistance(j, bmuIndex, gridCols);
                    if (distanceToBMU <= radius)
                    {
                        double influence = Math.Exp(-Math.Pow(distanceToBMU, 2) / (2 * Math.Pow(radius, 2)));
                        for (int d = 0; d < dimensions; d++)
                        {
                            centroids[j][d] += influence * learningRate * (data[i][d] - centroids[j][d]);
                        }
                    }
                }
            }
        }

        // Assign labels (closest centroid for each data point)
        for (int i = 0; i < numPoints; i++)
        {
            labels[i] = FindBMU(data[i], centroids);
        }

        return (labels, centroids);
    }

    private static int FindBMU(double[] point, double[][] centroids)
    {
        int bmuIndex = -1;
        double minDistance = double.MaxValue;

        for (int i = 0; i < centroids.Length; i++)
        {
            double distance = EuclideanDistance(point, centroids[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                bmuIndex = i;
            }
        }

        return bmuIndex;
    }

    private static double GridDistance(int index1, int index2, int gridCols)
    {
        int x1 = index1 / gridCols, y1 = index1 % gridCols;
        int x2 = index2 / gridCols, y2 = index2 % gridCols;
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
    }

    private static double EuclideanDistance(double[] point1, double[] point2)
    {
        return Math.Sqrt(point1.Zip(point2, (x, y) => Math.Pow(x - y, 2)).Sum());
    }

    public static double CalculateRMSE(List<double[]> data, int[] labels, double[][] centroids)
    {
        double totalSquaredError = 0.0;
        int numPoints = data.Count;

        for (int i = 0; i < numPoints; i++)
        {
            int cluster = labels[i];
            double squaredError = EuclideanDistanceSquared(data[i], centroids[cluster]);
            totalSquaredError += squaredError;
        }

        double meanSquaredError = totalSquaredError / numPoints;
        return Math.Sqrt(meanSquaredError); // RMSE
    }

    private static double EuclideanDistanceSquared(double[] point1, double[] point2)
    {
        return point1.Zip(point2, (x, y) => Math.Pow(x - y, 2)).Sum();
    }

    public static void SaveLabels(string filePath, int[] labels)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < labels.Length; i++)
            {
                writer.WriteLine($"{i},{labels[i]}"); // Format: index,label
            }
        }
    }

    public static void SaveCentroids(string filePath, double[][] centroids)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var centroid in centroids)
            {
                writer.WriteLine(string.Join(",", centroid)); // Format: centroid1,centroid2,...
            }
        }
    }

    public static void AppendRMSE(string filePath, double rmse)
    {
        using (StreamWriter writer = new StreamWriter(filePath, append: true))
        {
            writer.WriteLine(rmse); // Append the RMSE value as a new row
        }
    }
}
