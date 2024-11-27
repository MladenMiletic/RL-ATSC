using System;
using System.Collections.Generic;
using System.Linq;

class KMeans
{
    public static (int[], double[][]) PerformKMeans(List<double[]> data, int k, int maxIterations = 1000)
    {
        int numPoints = data.Count;
        int dimensions = data[0].Length;

        // Randomly initialize centroids
        Random random = new Random(1);
        double[][] centroids = data.OrderBy(x => random.Next()).Take(k).ToArray();

        int[] labels = new int[numPoints];
        bool hasConverged = false;
        int iterations = 0;

        while (!hasConverged && iterations < maxIterations)
        {
            hasConverged = true;
            iterations++;

            // Assign each point to the nearest centroid
            for (int i = 0; i < numPoints; i++)
            {
                int closestCentroid = -1;
                double closestDistance = double.MaxValue;

                for (int j = 0; j < k; j++)
                {
                    double distance = EuclideanDistance(data[i], centroids[j]);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCentroid = j;
                    }
                }

                if (labels[i] != closestCentroid)
                {
                    hasConverged = false;
                    labels[i] = closestCentroid;
                }
            }

            // Recalculate centroids
            double[][] newCentroids = new double[k][];
            int[] pointsPerCentroid = new int[k];

            for (int j = 0; j < k; j++)
            {
                newCentroids[j] = new double[dimensions];
            }

            for (int i = 0; i < numPoints; i++)
            {
                int cluster = labels[i];
                pointsPerCentroid[cluster]++;
                for (int d = 0; d < dimensions; d++)
                {
                    newCentroids[cluster][d] += data[i][d];
                }
            }

            for (int j = 0; j < k; j++)
            {
                if (pointsPerCentroid[j] > 0)
                {
                    for (int d = 0; d < dimensions; d++)
                    {
                        newCentroids[j][d] /= pointsPerCentroid[j];
                    }
                }
                else
                {
                    // Handle empty clusters by reinitializing the centroid
                    newCentroids[j] = data[random.Next(numPoints)];
                }
            }

            // Check if centroids have changed
            hasConverged = hasConverged && centroids.Zip(newCentroids, EuclideanDistance).All(d => d < 1e-6);
            centroids = newCentroids;
        }

        return (labels, centroids);
    }

    private static double EuclideanDistance(double[] point1, double[] point2)
    {
        return Math.Sqrt(point1.Zip(point2, (x, y) => Math.Pow(x - y, 2)).Sum());
    }

    public static void SaveLabels(string filePath, int[] labels)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < labels.Length; i++)
            {
                writer.WriteLine($"{labels[i]}"); // Format: index,label
            }
        }
    }

    // Save centroids to a CSV file
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
    public static void AppendRMSE(string filePath, double rmse)
    {
        using (StreamWriter writer = new StreamWriter(filePath, append: true))
        {
            writer.WriteLine(rmse); // Append the RMSE value as a new row
        }
    }
}
