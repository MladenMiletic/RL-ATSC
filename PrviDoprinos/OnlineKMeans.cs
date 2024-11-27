class OnlineKMeans
{
    public static (int[], double[][]) PerformOnlineKMeans(List<double[]> data, int k, int maxIterations = 100)
    {
        int numPoints = data.Count;
        int dimensions = data[0].Length;

        // Randomly initialize centroids
        Random random = new Random(1);
        double[][] centroids = data.OrderBy(x => random.Next()).Take(k).ToArray();

        int[] labels = new int[numPoints];
        int[] clusterCounts = new int[k]; // Tracks the number of points in each cluster

        // Perform Online K-Means
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            for (int i = 0; i < numPoints; i++)
            {
                // Assign to the nearest centroid
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

                labels[i] = closestCentroid;

                // Update the centroid with dynamic learning rate (1/n)
                int cluster = closestCentroid;
                clusterCounts[cluster]++;
                double learningRate = 1.0 / clusterCounts[cluster];

                for (int d = 0; d < dimensions; d++)
                {
                    centroids[cluster][d] = (1 - learningRate) * centroids[cluster][d] + learningRate * data[i][d];
                }
            }
        }

        return (labels, centroids);
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

    private static double EuclideanDistance(double[] point1, double[] point2)
    {
        return Math.Sqrt(point1.Zip(point2, (x, y) => Math.Pow(x - y, 2)).Sum());
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
